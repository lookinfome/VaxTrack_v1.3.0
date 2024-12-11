using v1Remastered.Dto;
using v1Remastered.Models;

namespace v1Remastered.Services
{
    public interface IAdminV2Service
    {
        // dash-actions
        public List<AdminDetailsDto_HospitalsList> FetchHospitalsList();
        public List<AdminDetailsDto_UserWithPendingApproval> FetchPendingApprovalsList();
        public bool ApproveSlotBookV2(string userId, string bookingId);
        public void UpdateAvailableSlotsById(string hospitalId, int increaseBy);

        // dash-usages
        public int FetchTotalUserCount();
        public int FetchTotalMaleUsersCount();
        public int FetchTotalFemaleUsersCount();
        public int FetchTotalVaccinatedUsersCount();
        public int FetchTotalUserWithSlotBookedCount();
        public int FetchTotalPartiallyVaccinatedUsersCount();
        

    }

    public class AdminV2Service:IAdminV2Service
    {
        private readonly AppDbContext _v1RemDb;
        private readonly IUserProfileService _userProfileService;
        private readonly IBookingService _bookingService;
        private readonly IHospitalService _hospitalService;
        private readonly IUserVaccineDetailsService _userVaccineDetailsService;
        public AdminV2Service(
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

        // service method: to fetch list of hospitals with all details
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
                            HospitalLocation = record.HospitalLocation,
                            HospitalAvailableSlots = record.HospitalAvailableSlots    
                        }
                    );
                }

                return fetchedHospitalsList;
            }
            return new List<AdminDetailsDto_HospitalsList>();
        }

        // service method: to fetch list of users with pending approval
        public List<AdminDetailsDto_UserWithPendingApproval> FetchPendingApprovalsList()
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
        
        // service method: to approve booked slots
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
    
        // service method: to update hospital slots
        public void UpdateAvailableSlotsById(string hospitalId, int increaseBy)
        {
            string _hospitalId = $"HOSP{hospitalId}";
            HospitalDetailsModel _hospitalDetails = _hospitalService.FetchHospitalDetailsById(_hospitalId);
            if(!string.IsNullOrEmpty(_hospitalDetails.HospitalId))
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
                Console.WriteLine($"--------Hospital available slots updataion failed with information hospitalId: {hospitalId}, increaseBy: {increaseBy}---------");
            }
        }  
    
    
        // service method: to fetch total user count
        public int FetchTotalUserCount()
        {
            var fetchedDetails = _v1RemDb.UserDetails.ToList();
            if(fetchedDetails != null)
            {
                return fetchedDetails.Count();
            }

            return 0;
        }

        // service method: to fetch total user count with slot booked
        public int FetchTotalUserWithSlotBookedCount()
        {
            var fetchedDetails = _v1RemDb.BookingDetails.ToList();
            if(fetchedDetails != null)
            {
                return fetchedDetails.Count();
            }

            return 0;
        } 
    
        // sevrice method: to fetch total male user count
        public int FetchTotalMaleUsersCount()
        {
            var fetchedDetails = _v1RemDb.UserDetails.Where(record => record.UserGender.ToUpper() == "M");
            if(fetchedDetails != null)
            {
                return fetchedDetails.Count();
            }
            return 0;
        }

        // sevrice method: to fetch total female user count
        public int FetchTotalFemaleUsersCount()
        {
            var fetchedDetails = _v1RemDb.UserDetails.Where(record => record.UserGender.ToUpper() == "F");
            if(fetchedDetails != null)
            {
                return fetchedDetails.Count();
            }
            return 0;
        }   

        // service method: to fetch total vaccinated user count
        public int FetchTotalVaccinatedUsersCount()
        {
            var fetchedDetails = _v1RemDb.BookingDetails
                                .Where(record=>record.Dose1ApproveDate != DateTime.MinValue && record.Dose2ApproveDate != DateTime.MinValue).ToList();
            if(fetchedDetails != null)
            {
                return fetchedDetails.Count();
            }
            return 0;
        }
        public int FetchTotalPartiallyVaccinatedUsersCount()
        {
            var fetchedDetails = _v1RemDb.BookingDetails
                                .Where(record=>record.Dose1ApproveDate != DateTime.MinValue && record.Dose2ApproveDate == DateTime.MinValue).ToList();
            if(fetchedDetails != null)
            {
                return fetchedDetails.Count();
            }
            return 0;
        
        }
    
    }
}