using System.Text;
using System.Text.Json;
using InternetCafeManagementSystem.Models;
using InternetCafeManagementSystem.Services;

namespace InternetCafeManagementSystem.Api
{
    /// <summary>
    /// CafeApiServer — Minimal HTTP Web API for SkySoft Internet Cafe.
    ///
    /// Exposes the existing InternetCafeService methods as REST endpoints
    /// so the customer-facing frontend (customer-portal.html) can read and
    /// write live data from the SQL database.
    ///
    /// Runs on http://localhost:5000 in a background thread alongside the
    /// existing console app — no changes to business logic required.
    ///
    /// Endpoints (map directly to console app options):
    ///   GET  /api/customers              → All customers         (Option 3/4)
    ///   GET  /api/customers/{id}         → Customer by ID        (Option 3)
    ///   GET  /api/customers/search?name= → Search by name        (Option 4)
    ///   POST /api/customers              → Add customer          (Option 10)
    ///   POST /api/customers/{id}/topup  → Top up balance        (Option 9)
    ///   GET  /api/sessions               → All sessions          (Option 5)
    ///   GET  /api/sessions/active        → Active sessions       (Option 6)
    ///   GET  /api/sessions/history/{id}  → Session history       (Option 8)
    ///   POST /api/sessions/start         → Start session         (Option 1)
    ///   POST /api/sessions/end           → End session           (Option 2)
    ///   GET  /api/pcs                    → PC availability       (Option 7)
    ///   GET  /api/stats                  → Dashboard stats
    /// </summary>
    public static class CafeApiServer
    {
        private static InternetCafeService _cafe = null!;

        private static readonly JsonSerializerOptions _json = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };

        public static void Start(InternetCafeService cafe)
        {
            _cafe = cafe;

            var listener = new System.Net.HttpListener();
            listener.Prefixes.Add("http://localhost:5000/");
            listener.Start();

            Console.WriteLine("API listening on http://localhost:5000/");

            while (true)
            {
                try
                {
                    var context = listener.GetContext();
                    // Handle each request in its own thread so the API stays responsive
                    ThreadPool.QueueUserWorkItem(_ => HandleRequest(context));
                }
                catch { /* listener stopped */ }
            }
        }

        private static void HandleRequest(System.Net.HttpListenerContext ctx)
        {
            var req  = ctx.Request;
            var resp = ctx.Response;

            // ── CORS headers ─────────────────────────────────────────────
            // Required so the HTML frontend (opened as a file or different port)
            // is allowed to call this API from the browser.
            resp.Headers.Add("Access-Control-Allow-Origin", "*");
            resp.Headers.Add("Access-Control-Allow-Methods", "GET, POST, OPTIONS");
            resp.Headers.Add("Access-Control-Allow-Headers", "Content-Type");

            // Handle preflight OPTIONS request from browser
            if (req.HttpMethod == "OPTIONS")
            {
                resp.StatusCode = 204;
                resp.Close();
                return;
            }

                string path = req.Url?.AbsolutePath.TrimEnd('/') ?? "";
                string pathLower = path.ToLower();            
                string method = req.HttpMethod;

            try
            {
                // ── Route matching ────────────────────────────────────────

                // GET /api/stats — dashboard summary numbers
                if (method == "GET" && path == "/api/stats")
                {
                    var allCustomers = _cafe.SearchCustomersByName(""); // returns all
                    var allSessions  = _cafe.GetAllSessions();
                    var activeSessions = _cafe.GetActiveSessions();
                    var allPcs       = _cafe.GetAllPCs();

                    int totalCustomers  = 0;
                    foreach (var _ in allCustomers) totalCustomers++;

                    int totalSessions = 0;
                    foreach (var _ in allSessions) totalSessions++;

                    int availablePcs = 0;
                    foreach (var pc in allPcs)
                        if (pc.IsAvailable) availablePcs++;

                    var stats = new
                    {
                        totalCustomers,
                        activeSessions  = activeSessions.Count,
                        availablePcs,
                        totalSessions
                    };

                    SendJson(resp, 200, stats);
                    return;
                }

                // GET /api/customers/search?name=xxx — search by name (Option 4)
                if (method == "GET" && path == "/api/customers/search")
                {
                    string name = req.QueryString["name"] ?? "";
                    var results = _cafe.SearchCustomersByName(name);
                    SendJson(resp, 200, results.Select(c => MapCustomer(c)));
                    return;
                }

                // GET /api/customers — all customers (for dropdowns etc.)
                if (method == "GET" && path == "/api/customers")
                {
                    var all = _cafe.SearchCustomersByName(""); // returns all when empty
                    SendJson(resp, 200, all.Select(c => MapCustomer(c)));
                    return;
                }

                // GET /api/customers/{id} — get single customer by ID (Option 3)
                if (method == "GET" && path.StartsWith("/api/customers/") && !path.Contains("/topup"))
                {
                string id = req.Url?.AbsolutePath.TrimEnd('/').Replace("/api/customers/", "") ?? "";                    try
                    {
                        var customer = _cafe.GetCustomer(id);
                        SendJson(resp, 200, MapCustomer(customer));
                    }
                    catch
                    {
                        SendJson(resp, 404, new { error = "Customer not found." });
                    }
                    return;
                }

                // POST /api/customers — add new customer (Option 10)
                if (method == "POST" && path == "/api/customers")
                {
                    var body = ReadBody(req);
                    var doc  = JsonDocument.Parse(body);

                    string id      = doc.RootElement.GetProperty("customerId").GetString() ?? "";
                    string name    = doc.RootElement.GetProperty("name").GetString() ?? "";
                    string email   = doc.RootElement.GetProperty("email").GetString() ?? "";
                    decimal balance = doc.RootElement.GetProperty("balance").GetDecimal();

                    if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(email))
                    {
                        SendJson(resp, 400, new { error = "Customer ID, name and email are required." });
                        return;
                    }

                    if (_cafe.CustomerExists(id))
                    {
                        SendJson(resp, 409, new { error = "Customer ID already exists." });
                        return;
                    }

                    var customer = new Customer(id, name, email, balance);
                    _cafe.AddCustomer(customer);
                    SendJson(resp, 201, MapCustomer(customer));
                    return;
                }

                // POST /api/customers/{id}/topup — top up balance (Option 9)
                if (method == "POST" && path.EndsWith("/topup"))
                {
                    string id   = path.Replace("/api/customers/", "").Replace("/topup", "");
                    var body    = ReadBody(req);
                    var doc     = JsonDocument.Parse(body);
                    decimal amt = doc.RootElement.GetProperty("amount").GetDecimal();

                    try
                    {
                        _cafe.TopUpBalance(id, amt);
                        var customer = _cafe.GetCustomer(id);
                        SendJson(resp, 200, MapCustomer(customer));
                    }
                    catch (Exception ex)
                    {
                        SendJson(resp, 400, new { error = ex.Message });
                    }
                    return;
                }

                // GET /api/sessions/active — active sessions only (Option 6)
                if (method == "GET" && path == "/api/sessions/active")
                {
                    var active = _cafe.GetActiveSessions();
                    SendJson(resp, 200, active.Select(s => MapSession(s)));
                    return;
                }

                // GET /api/sessions/history/{customerId} — session history (Option 8)
                if (method == "GET" && path.StartsWith("/api/sessions/history/"))
                {
                    string id  = path.Replace("/api/sessions/history/", "");
                    var history = _cafe.GetSessionHistory(id);
                    SendJson(resp, 200, history.Select(s => MapSession(s)));
                    return;
                }

                // GET /api/sessions — all sessions (Option 5)
                if (method == "GET" && path == "/api/sessions")
                {
                    var all = _cafe.GetAllSessions();
                    SendJson(resp, 200, all.Select(s => MapSession(s)));
                    return;
                }

                // POST /api/sessions/start — start a session (Option 1)
                if (method == "POST" && path == "/api/sessions/start")
                {
                    var body       = ReadBody(req);
                    var doc        = JsonDocument.Parse(body);
                    string custId  = doc.RootElement.GetProperty("customerId").GetString() ?? "";

                    // Generate session ID the same way as the console app
                    string sessionId = "S" + DateTime.Now.ToString("HHmmss");

                    try
                    {
                        var session = _cafe.StartSession(sessionId, custId);
                        SendJson(resp, 201, MapSession(session));
                    }
                    catch (Exception ex)
                    {
                        SendJson(resp, 400, new { error = ex.Message });
                    }
                    return;
                }

                // POST /api/sessions/end — end a session (Option 2)
                if (method == "POST" && path == "/api/sessions/end")
                {
                    var body      = ReadBody(req);
                    var doc       = JsonDocument.Parse(body);
                    string sessId = doc.RootElement.GetProperty("sessionId").GetString() ?? "";

                    try
                    {
                        decimal cost = _cafe.EndSession(sessId);
                        SendJson(resp, 200, new { sessionId = sessId, cost });
                    }
                    catch (Exception ex)
                    {
                        SendJson(resp, 400, new { error = ex.Message });
                    }
                    return;
                }

                // GET /api/pcs — PC availability (Option 7)
                if (method == "GET" && path == "/api/pcs")
                {
                    var all = _cafe.GetAllPCs();
                    SendJson(resp, 200, all.Select(p => new
                    {
                        pcId      = p.PCID,
                        status    = p.IsAvailable ? "Available" : "Occupied",
                        hourlyRate = p.HourlyRate
                    }));
                    return;
                }

                // 404 — no route matched
                SendJson(resp, 404, new { error = "Endpoint not found." });
            }
            catch (Exception ex)
            {
                SendJson(resp, 500, new { error = ex.Message });
            }
        }

        // ── Helpers ──────────────────────────────────────────────────────

        // Maps a Customer model to a plain anonymous object for JSON serialisation.
        // We do this to control exactly what gets sent to the frontend,
        // and to avoid any serialisation issues with custom data structures.
        private static object MapCustomer(Customer c) => new
        {
            customerId = c.CustomerID,
            name       = c.Name,
            email      = c.Email,
            balance    = c.Balance
        };

        // Maps a Session model to a plain anonymous object for JSON serialisation.
        // Cost is calculated on the fly using the PC's hourly rate because Session
        // stores no Cost field — it uses CalculateCost(hourlyRate) instead.
        // We look up the PC from the service to get the correct rate.
        private static object MapSession(Session s)
        {
            decimal cost = 0;
            try
            {
                var pc = _cafe.GetAllPCs().Find(p => p.PCID == s.PCID);
                if (pc != null) cost = s.CalculateCost(pc.HourlyRate);
            }
            catch { /* if PC not found, cost stays 0 */ }

            return new
            {
                sessionId  = s.SessionID,
                customerId = s.CustomerID,
                pcId       = s.PCID,
                startTime  = s.StartTime.ToString("yyyy-MM-dd HH:mm"),
                endTime    = s.EndTime?.ToString("yyyy-MM-dd HH:mm"),
                cost
            };
        }

        private static void SendJson(System.Net.HttpListenerResponse resp, int status, object data)
        {
            string json  = JsonSerializer.Serialize(data, _json);
            byte[] bytes = Encoding.UTF8.GetBytes(json);
            resp.StatusCode      = status;
            resp.ContentType     = "application/json";
            resp.ContentLength64 = bytes.Length;
            resp.OutputStream.Write(bytes, 0, bytes.Length);
            resp.OutputStream.Close();
        }

        private static string ReadBody(System.Net.HttpListenerRequest req)
        {
            using var reader = new System.IO.StreamReader(req.InputStream, req.ContentEncoding);
            return reader.ReadToEnd();
        }
    }
}
