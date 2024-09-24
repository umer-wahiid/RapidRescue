using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using RapidResuce.Context;
using RapidResuce.Enums;
using RapidResuce.Interfaces;
using RapidResuce.Models;
using RapidResuce.Services;
using RapidResuce.SignalHub;

namespace RapidResuce.Controllers
{
    public class EmergencyRequestsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly INotificationService _notificationService;
        private readonly IHubContext<NotificationHub> _hubContext;

        public EmergencyRequestsController(ApplicationDbContext context, IHubContext<NotificationHub> hubContext, INotificationService notificationService)
        {
            _context = context;
            _hubContext = hubContext;
            _notificationService = notificationService;
        }

        // GET: EmergencyRequests
        public IActionResult List()
        {
            if (Session.UserId == 0)
                return RedirectToAction("Index", "Login");

            ViewData["Ambulances"] = new SelectList(_context.Ambulances, "Id", "VehicleNumber");
            ViewData["Users"] = new SelectList(_context.Users.Where(u => u.Role == UserRole.Patient), "Id", "LastName");

            return View();
        }

        // GET: Assigned EmergencyRequests
        public IActionResult AssignedList()
        {
            if (Session.UserId == 0)
                return RedirectToAction("Index", "Login");

            ViewData["Ambulances"] = new SelectList(_context.Ambulances, "Id", "VehicleNumber");
            ViewData["Users"] = new SelectList(_context.Users.Where(u => u.Role == UserRole.Patient), "Id", "LastName");

            return View();
        }

        public IActionResult Index()
        {
            if (Session.UserId == 0)
                return RedirectToAction("Index", "Login");

            bool exists = _context.EmergencyRequests.Any(er => er.UserId == Session.UserId && er.Status != RequestStatus.Canceled && er.Status != RequestStatus.Completed);

            if (exists)
            {
                return RedirectToAction("RequestTracking");
            }
            else
            {
                ViewData["Symptoms"] = new SelectList(_context.FirstAids, "Id", "symptoms");

                return View();
            }
        }

        public IActionResult RequestTracking()
        {
            if (Session.UserId == 0)
                return RedirectToAction("Index", "Login");

            var request = _context.EmergencyRequests.Include(re => re.FirstAid).FirstOrDefault(re => re.Status != RequestStatus.Canceled && re.Status != RequestStatus.Completed && re.UserId == Session.UserId);

            if (request is null)
                return RedirectToAction("Index", "EmergencyRequests");

            if (request?.AmbulanceId is not null)
                request.Ambulance = _context.Ambulances.Include(amb => amb.Driver).FirstOrDefault(amb => amb.Id == request.AmbulanceId);

            return View(request);
        }

        public async Task<IActionResult> Get()
        {
            try
            {
                dynamic requests;
                if (Session.Role == UserRole.Admin)
                {
                    requests = await _context.EmergencyRequests.Include(e => e.Ambulance)
                                            .Include(e => e.User)
                                            .Select(e => new
                                            {
                                                e.Id,
                                                e.PickupAddress,
                                                e.HospitalName,
                                                e.EmergencyContact,
                                                e.MedicalInfo,
                                                e.EquipmentLevel,
                                                e.AmbulanceId,
                                                e.Type,
                                                e.Status,
                                                e.CreatedDate,
                                                e.UpdatedDate,
                                                Ambulance = e.Ambulance != null ? new { e.Ambulance.VehicleNumber } : null,
                                                User = new { e.User.FirstName, e.User.LastName }
                                            }).AsNoTracking().ToListAsync();
                }
                else if (Session.Role == UserRole.Driver)
                {
                    requests = await _context.EmergencyRequests.Include(e => e.Ambulance)
                                            .Include(e => e.User)
                                            .Select(e => new
                                            {
                                                e.Id,
                                                e.PickupAddress,
                                                e.HospitalName,
                                                e.EmergencyContact,
                                                e.MedicalInfo,
                                                e.EquipmentLevel,
                                                e.AmbulanceId,
                                                e.Type,
                                                e.Status,
                                                e.CreatedDate,
                                                e.UpdatedDate,
                                                Ambulance = e.Ambulance != null ? new { e.Ambulance.VehicleNumber, e.Ambulance.DriverId, e.Ambulance.EmtId } : null,
                                                User = new { e.User.FirstName, e.User.LastName }
                                            }).Where(e => e.Ambulance.DriverId == Session.UserId).AsNoTracking().ToListAsync();
                }
                else
                {
                    requests = await _context.EmergencyRequests.Include(e => e.Ambulance)
                                            .Include(e => e.User)
                                            .Select(e => new
                                            {
                                                e.Id,
                                                e.PickupAddress,
                                                e.HospitalName,
                                                e.EmergencyContact,
                                                e.MedicalInfo,
                                                e.EquipmentLevel,
                                                e.AmbulanceId,
                                                e.Type,
                                                e.Status,
                                                e.CreatedDate,
                                                e.UpdatedDate,
                                                Ambulance = e.Ambulance != null ? new { e.Ambulance.VehicleNumber, e.Ambulance.DriverId, e.Ambulance.EmtId } : null,
                                                User = new { e.User.FirstName, e.User.LastName }
                                            }).Where(e => e.Ambulance.EmtId == Session.UserId).AsNoTracking().ToListAsync();
                }
                return Json(new { Success = true, Data = requests });
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<IActionResult> GetById(int id)
        {
            try
            {

                var request = await _context.EmergencyRequests.Include(e => e.Ambulance)
                                        .Include(e => e.User)
                                        .Select(e => new
                                        {
                                            e.Id,
                                            e.UserId,
                                            e.PickupAddress,
                                            e.HospitalName,
                                            e.EmergencyContact,
                                            e.MedicalInfo,
                                            e.EquipmentLevel,
                                            e.Type,
                                            e.AmbulanceId,
                                            e.Status,
                                            e.CreatedDate,
                                            e.UpdatedDate,
                                            Ambulance = e.Ambulance != null ? new { e.Ambulance.VehicleNumber } : null,
                                            User = new { e.User.FirstName, e.User.LastName }
                                        }).AsNoTracking()
                                        .FirstOrDefaultAsync(m => m.Id == id);
                if (request is null)
                    return Json(new { Success = false, Message = $"Request with id {id} not found" });

                return Json(new { Success = true, Data = request });
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpPost]
        public async Task<IActionResult> Add(EmergencyRequest emergencyRequest)
        {
            try
            {
                bool exists = _context.EmergencyRequests.Any(er => er.UserId == Session.UserId && er.Status != RequestStatus.Canceled && er.Status != RequestStatus.Completed);

                if (exists)
                {
                    return Json(new { Success = true, Message = $"Your request is already in progress" });
                }
                emergencyRequest.UserId = Session.UserId;
                ModelState.Remove("User");
                ModelState.Remove("Ambulance");
                ModelState.Remove("FirstAid");
                if (ModelState.IsValid)
                {
                    emergencyRequest.CreatedDate = DateTime.Now;
                    emergencyRequest.Status = RequestStatus.Pending;
                    _context.Add(emergencyRequest);
                    await _context.SaveChangesAsync();
                    // Send notification to the admin
                    int adminId = _context.Users.FirstOrDefault(us => us.Role == UserRole.Admin).Id;
                    await _hubContext.Clients.All.SendAsync("ReceiveNotification", adminId, UserRole.Admin.ToString(), "New request received !");
                    _notificationService.SaveNotificationToDatabase(adminId, UserRole.Admin, "New request received");
                    return Json(new { Success = true, Message = $"Service Requested Succesfully" });
                }
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                    .Select(e => e.ErrorMessage)
                                    .ToList();

                return Json(new { Success = false, Message = string.Join(", ", errors) });

            }
            catch (Exception ex)
            {
                var e = ex.InnerException?.Message;
                return Json(new { Success = false, Message = $"{ex.Message}" });
            }

        }

        [HttpPost]
        public async Task<IActionResult> Edit(EmergencyRequest request)
        {
            try
            {

                if (request.Id == 0)
                    return Json(new { Success = false, Message = $"Request not found" });

                var existingRequest = _context.EmergencyRequests.AsNoTracking().FirstOrDefault(m => m.Id == request.Id);

                if (existingRequest is null)
                    return Json(new { Success = false, Message = $"Request not found" });


                existingRequest.UpdatedDate = DateTime.Now;
                existingRequest.Status = request.Status;
                existingRequest.AmbulanceId = request.AmbulanceId;

                var amb = _context.Ambulances.FirstOrDefault(amb => amb.Id == request.AmbulanceId);

                int? driverId = amb.DriverId;
                int? emtId = amb.EmtId;
                int? userId = existingRequest.UserId;

                try
                {
                    _context.Update(existingRequest);
                    await _context.SaveChangesAsync();

                    if (userId.HasValue)
                    {
                        // Send notification to the requester user
                        await _hubContext.Clients.All.SendAsync("ReceiveNotification", userId, UserRole.Patient.ToString(), "Your request's status is set to  " + existingRequest.Status.ToString());
                        _notificationService.SaveNotificationToDatabase(userId, UserRole.Patient, "Your request's status is set to  " + existingRequest.Status.ToString());
                    }

                    if (driverId.HasValue)
                    {
                        // Send notification to the ambulance driver
                        await _hubContext.Clients.All.SendAsync("ReceiveNotification", driverId, UserRole.Driver.ToString(), "New request assigned");
                        _notificationService.SaveNotificationToDatabase(driverId, UserRole.Driver, "New request assigned");
                    }

                    if (emtId.HasValue)
                    {
                        // Send notification to the ambulance driver
                        await _hubContext.Clients.All.SendAsync("ReceiveNotification", emtId, UserRole.EMT.ToString(), "New request assigned");
                        _notificationService.SaveNotificationToDatabase(emtId, UserRole.EMT, "New request assigned");
                    }

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmergencyRequestExists(request.Id))
                        return NotFound();
                    else
                        throw;
                }
                return Json(new { Success = true, Message = $"Request Updated Succesfully" });
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = $"{ex.Message}" });
            }
        }

        private bool EmergencyRequestExists(int id)
        {
            return _context.EmergencyRequests.Any(e => e.Id == id);
        }

        [HttpPost]
        public async Task ClearNotifications()
        {
            var notification = await _context.Notifications.Where(ntfy => ntfy.UserId == Session.UserId && ntfy.IsRead == false).ToListAsync();
            if (notification.Count() > 0)
            {
                foreach (var item in notification)
                {
                    item.IsRead = true;
                    _context.Update(item);
                    await _context.SaveChangesAsync();
                }
            }
        }
    }
}
