

using v1Remastered.Dto;
using v1Remastered.Models;

namespace v1Remastered.Services
{
    public interface IAdminService
    {
        // action
        public List<AdminDetailsDto_UserWithPendingApproval> FetchUsersWithPendingApproval();
        public bool ApproveSlotBookV2(string userId, string bookingId);
        public bool ApproveSlotBook(string userId, string bookingId);
        public List<AdminDetailsDto_HospitalsList> FetchHospitalsList();
        public void UpdateAvailableSlotsById(string hospitalId, int increaseBy);

        // status
        public int FetchTotalUsersCount();
        public int FetchTotalUsersCountWithSlotBooked();
        public int FetchTotalUsersCountWithApprovedSlots();
        public int FetchTotalMaleUsersCount();
        public int FetchTotalFemaleUsersCount();
        public List<AdminDetailsDto_BookingMonthCount> FetchDose1BookingMonthCount();
        public List<AdminDetailsDto_BookingMonthCount> FetchDose2BookingMonthCount();

    }
    public class AdminService : IAdminService
    {
        private readonly AppDbContext _v1RemDb;
        private readonly IUserProfileService _userProfileService;
        private readonly IBookingService _bookingService;
        private readonly IHospitalService _hospitalService;
        private readonly IUserVaccineDetailsService _userVaccineDetailsService;

        public AdminService(
            AppDbContext v1RemDb,
            IBookingService bookingService,
            IHospitalService hospitalService,
            IUserProfileService userProfileService,
            IUserVaccineDetailsService userVaccineDetailsService
        )
        {
            _v1RemDb = v1RemDb;
            _bookingService = bookingService;
            _hospitalService = hospitalService;
            _userProfileService = userProfileService;
            _userVaccineDetailsService = userVaccineDetailsService;
        }

        // user with pending approval
        public List<AdminDetailsDto_UserWithPendingApproval> FetchUsersWithPendingApproval()
        {
            var usersInfo = from userDetails in _v1RemDb.UserDetails
                            join userVaccinationDetails in _v1RemDb.UserVaccineDetails
                            on userDetails.UserId equals userVaccinationDetails.UserId
                            join bookingDetails in _v1RemDb.BookingDetails
                            on userVaccinationDetails.UserVaccinationId equals bookingDetails.UserVaccinationId
                            where (bookingDetails.Dose1BookDate != DateTime.MinValue && bookingDetails.Dose1ApproveDate == DateTime.MinValue) ||
                            (bookingDetails.Dose2BookDate != DateTime.MinValue && bookingDetails.Dose2ApproveDate == DateTime.MinValue)
                            select new
                            {
                                userVaccinationDetails.UserId,
                                userDetails.UserName,
                                userVaccinationDetails.UserVaccinationStatus,
                                bookingDetails.BookingId,
                                bookingDetails.Dose1BookDate,
                                bookingDetails.Dose2BookDate,
                                bookingDetails.D1HospitalId,
                                bookingDetails.D2HospitalId
                            };

            var usersList = usersInfo.ToList().Select(user => new AdminDetailsDto_UserWithPendingApproval
            {
                UserId = user.UserId,
                Username = user.UserName,
                UserVaccinationStatus = user.UserVaccinationStatus,
                BookingId = user.BookingId,
                Dose1Date = user.Dose1BookDate,
                Dose2Date = user.Dose2BookDate,
                D1HospitalName = _hospitalService.FetchHospitalNameById(user.D1HospitalId),
                D2HospitalName = _hospitalService.FetchHospitalNameById(user.D2HospitalId)
            }).ToList();

            return usersList.Count > 0 ? usersList : new List<AdminDetailsDto_UserWithPendingApproval>();
        }

        // approve booked slot v2
        public bool ApproveSlotBookV2(string userId, string bookingId)
        {
            // fetch booking details
            var bookingDetails = _v1RemDb.BookingDetails.FirstOrDefault(record=>record.BookingId == bookingId);

            if(bookingDetails != null)
            {
                // record approval date and time
                DateTime approvalDateTime = DateTime.UtcNow;

                string hospitalId = "";


                if(bookingDetails.Dose1BookDate != DateTime.MinValue && bookingDetails.Dose1ApproveDate == DateTime.MinValue)
                {
                    bookingDetails.Dose1ApproveDate = approvalDateTime;
                    hospitalId = bookingDetails.D1HospitalId;
                }

                if(bookingDetails.Dose2BookDate != DateTime.MinValue && bookingDetails.Dose2ApproveDate == DateTime.MinValue)
                {
                    bookingDetails.Dose2ApproveDate = approvalDateTime;
                    hospitalId = bookingDetails.D2HospitalId;
                }

                _v1RemDb.BookingDetails.Update(bookingDetails);
                int bookingDetailsUpdateStatus = _v1RemDb.SaveChanges();

                if(bookingDetailsUpdateStatus > 0)
                {
                    // fetch user vaccination details
                    var userVaccineDetails = _v1RemDb.UserVaccineDetails.FirstOrDefault(record=>record.UserId == userId);

                    if(userVaccineDetails != null)
                    {
                        // update user vaccination status
                        userVaccineDetails.UserVaccinationStatus = userVaccineDetails.UserVaccinationStatus+1;

                        // save to DB
                        _v1RemDb.UserVaccineDetails.Update(userVaccineDetails);
                        int userVaccinationDetailsUpdateStatus = _v1RemDb.SaveChanges();

                        if(userVaccinationDetailsUpdateStatus > 0)
                        {
                            // fetch hospital details
                            var hospitalDetails = _v1RemDb.HospitalDetails.FirstOrDefault(record=>record.HospitalId == hospitalId);

                            if(hospitalDetails != null)
                            {
                                hospitalDetails.HospitalAvailableSlots = hospitalDetails.HospitalAvailableSlots+1;

                                _v1RemDb.HospitalDetails.Update(hospitalDetails);
                                _v1RemDb.SaveChanges();

                                return true;

                            }

                            else  {return false;} // no hospital details found
                        }

                        else {return false;} // user vaccination details not update

                    }

                    else {return false;} // no user vaccination details found
                }

                else {return false;} // booking details update failed

            }
            else{return false;} // no booking details found
        }
        
        // approve booked slot
        public bool ApproveSlotBook(string userId, string bookingId)
        {
            // fetch booking details
            var bookingDetails = _v1RemDb.BookingDetails.FirstOrDefault(record=>record.BookingId == bookingId);

            if(bookingDetails != null)
            {
                // record approval date and time
                DateTime approvalDateTime = DateTime.UtcNow;

                // update approval dates for d1 and d2
                bookingDetails.Dose1ApproveDate = approvalDateTime;
                bookingDetails.Dose2ApproveDate = approvalDateTime;

                // save to DB
                _v1RemDb.BookingDetails.Update(bookingDetails);
                int bookingDetailsUpdateStatus = _v1RemDb.SaveChanges();

                if(bookingDetailsUpdateStatus > 0)
                {
                    // fetch user vaccination details
                    var userVaccineDetails = _v1RemDb.UserVaccineDetails.FirstOrDefault(record=>record.UserId == userId);

                    if(userVaccineDetails != null)
                    {
                        // update user vaccination status
                        userVaccineDetails.UserVaccinationStatus = userVaccineDetails.UserVaccinationStatus+1;

                        // save to DB
                        _v1RemDb.UserVaccineDetails.Update(userVaccineDetails);
                        int userVaccinationDetailsUpdateStatus = _v1RemDb.SaveChanges();

                        if(userVaccinationDetailsUpdateStatus > 0)
                        {
                            if(bookingDetails.D1HospitalId == bookingDetails.D2HospitalId)
                            {
                                // fetch hospital details
                                var hospitalDetails = _v1RemDb.HospitalDetails.FirstOrDefault(record=>record.HospitalId == bookingDetails.D1HospitalId);

                                if(hospitalDetails != null)
                                {
                                    hospitalDetails.HospitalAvailableSlots = hospitalDetails.HospitalAvailableSlots+2;

                                    _v1RemDb.HospitalDetails.Update(hospitalDetails);
                                    _v1RemDb.SaveChanges();

                                }
                            }
                            else
                            {
                                // fetch hospital details
                                var hospital1Details = _v1RemDb.HospitalDetails.FirstOrDefault(record=>record.HospitalId == bookingDetails.D1HospitalId);
                                var hospital2Details = _v1RemDb.HospitalDetails.FirstOrDefault(record=>record.HospitalId == bookingDetails.D2HospitalId);
                                if(hospital1Details!= null && hospital2Details != null)
                                {
                                    hospital1Details.HospitalAvailableSlots = hospital1Details.HospitalAvailableSlots+1;
                                    hospital2Details.HospitalAvailableSlots = hospital2Details.HospitalAvailableSlots+1;

                                    _v1RemDb.HospitalDetails.Update(hospital1Details);
                                    _v1RemDb.SaveChanges();

                                    _v1RemDb.HospitalDetails.Update(hospital2Details);
                                    _v1RemDb.SaveChanges();

                                }
                            }

                            return true;
                            
                            
                        }
                        else
                        {
                            Console.WriteLine($"--------user vaccination details updation failed---------");
                            return false;
                        }

                    }
                    else
                    {
                        Console.WriteLine($"--------user vaccination details not found---------");
                        return false;
                    }

                }
                else
                {
                    Console.WriteLine($"---------booking details updation failed---------");
                    return false;
                }


            }
            else
            {
                // no booking details found
                Console.WriteLine($"---------no booking details found---------");
                return false;
            }





                // update hospital available slots

            
        } 

        // fetch hospital details with name and avaialable slots
        public List<AdminDetailsDto_HospitalsList> FetchHospitalsList()
        {
            List<AdminDetailsDto_HospitalsList> fetchedHospitalsList = new List<AdminDetailsDto_HospitalsList>();
            var hospitalsList = _hospitalService.FetchHospitalsList();
            if(hospitalsList != null)
            {
                foreach (var record in hospitalsList)
                {
                    fetchedHospitalsList.Add(
                        new AdminDetailsDto_HospitalsList
                        {
                            HospitalId = record.HospitalId,
                            HospitalName = record.HospitalName,
                            HospitalAvailableSlots = record.HospitalAvailableSlots    
                        }
                    );
                }

                return fetchedHospitalsList;
            }
            return new List<AdminDetailsDto_HospitalsList>();
        }
    
        // update hospital avialable slots
        public void UpdateAvailableSlotsById(string hospitalId, int increaseBy)
        {
            HospitalDetailsModel _hospitalDetails = _hospitalService.FetchHospitalDetailsById(hospitalId);
            if(_hospitalDetails.HospitalId != null)
            {
                int updatedSlots = _hospitalDetails.HospitalAvailableSlots + increaseBy;
                if(updatedSlots < 0)
                {
                    _hospitalDetails.HospitalAvailableSlots = 0;
                }
                else
                {
                    _hospitalDetails.HospitalAvailableSlots = updatedSlots;
                }
                
                _v1RemDb.HospitalDetails.Update(_hospitalDetails);
                _v1RemDb.SaveChanges();
            }
            else
            {
                Console.WriteLine($"--------Hospital available slots updataion failed---------");
            }
        } 
    
    
        // fetch total users count on app
        public int FetchTotalUsersCount()
        {
            int totalUsersCount = 0;
            var fetchedData =_v1RemDb.UserDetails.ToList();
            if(fetchedData.Count > 0)
            {
                totalUsersCount = fetchedData.Count();
            }

            return totalUsersCount;
        }

        // fetch total users count with slot booked
        public int FetchTotalUsersCountWithSlotBooked()
        {
            int totalUsersCount = 0;
            var fetchedData = _v1RemDb.BookingDetails.Where(record=>record.BookingId != null && record.BookingId != "").ToList();
            if(fetchedData.Count > 0)
            {
                totalUsersCount = fetchedData.Count();
            }

            return totalUsersCount;
        }

        // fetch total users count with slot appoved
        public int FetchTotalUsersCountWithApprovedSlots()
        {
            int totalUsersCount = 0;
            var fetchedData = _v1RemDb.BookingDetails
                                .Where(record=>record.Dose1ApproveDate != DateTime.MinValue && record.Dose2ApproveDate != DateTime.MinValue)
                                .ToList();

            if(fetchedData.Count > 0)
            {
                totalUsersCount = fetchedData.Count;
            }

            return totalUsersCount;
        }
    
        // fetch total male users gender count
        public int FetchTotalMaleUsersCount()
        {
            int totalUsersCount = 0;
            var fetchedData = _v1RemDb.UserDetails.Where(record=>record.UserGender == "M").ToList();
            if(fetchedData != null)
            {
                totalUsersCount = fetchedData.Count;
            }

            return totalUsersCount;
        }

        // fetch total female users gender count
        public int FetchTotalFemaleUsersCount()
        {
            int totalUsersCount = 0;
            var fetchedData = _v1RemDb.UserDetails.Where(record=>record.UserGender == "F").ToList();
            if(fetchedData != null)
            {
                totalUsersCount = fetchedData.Count;
            }

            return totalUsersCount;
        }

        // fetch dose 1 booking count 
        public List<AdminDetailsDto_BookingMonthCount> FetchDose1BookingMonthCount()
        {
            var fetchedData = _v1RemDb.BookingDetails
                                .GroupBy(record=> new
                                {
                                    record.Dose1BookDate.Year, record.Dose1BookDate.Month
                                })
                                .Select(record=> new AdminDetailsDto_BookingMonthCount
                                {
                                    BookingMonth = new DateTime(record.Key.Year, record.Key.Month, 1).ToString("yyyy-MM"),
                                    BookingCount = record.Count()
                                }).ToList();

            return fetchedData;
        }

        // fetch dose 2 booking count
        public List<AdminDetailsDto_BookingMonthCount> FetchDose2BookingMonthCount()
        {
            var fetchedData = _v1RemDb.BookingDetails
                                .GroupBy(record=> new
                                {
                                    record.Dose1BookDate.Year, record.Dose2BookDate.Month
                                })
                                .Select(record=> new AdminDetailsDto_BookingMonthCount
                                {
                                    BookingMonth = new DateTime(record.Key.Year, record.Key.Month, 1).ToString("yyyy-MM"),
                                    BookingCount = record.Count()
                                }).ToList();

            return fetchedData;
        }
    
    }
}