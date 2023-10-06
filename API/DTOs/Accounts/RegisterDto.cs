using API.Models;
using API.Utilities.Enums;

namespace API.DTOs.Accounts
{
    public class RegisterDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
        public GenderLevel Gender { get; set; }
        public DateTime HiringDate { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Major {  get; set; }
        public string Degree { get; set; }
        public float Gpa { get; set; }
        public string UniversityCode { get; set; }
        public string UniversityName { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }

        public static implicit operator Account(RegisterDto accountDto)
        {
            return new Account
            {
                Guid = Guid.NewGuid(),
                Password = accountDto.ConfirmPassword,
                Otp = 0,
                IsUsed = true,
                ExpiredTime = DateTime.Now,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now
            };
        }

        public static implicit operator Employee(RegisterDto registrationDto)
        {
            return new Employee
            {
                FirstName = registrationDto.FirstName,
                LastName = registrationDto.LastName,
                BirthDate = registrationDto.BirthDate,
                Gender = registrationDto.Gender,
                HiringDate = registrationDto.HiringDate,
                Email = registrationDto.Email,
                PhoneNumber = registrationDto.PhoneNumber,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now
            };
        }
        public static implicit operator Education(RegisterDto registrationDto)
        {
            return new Education
            {
                Guid = Guid.NewGuid(),
                Major = registrationDto.Major,
                Degree = registrationDto.Degree,
                Gpa = registrationDto.Gpa,
                UniversityGuid = Guid.NewGuid(),
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now
            };
        }
        public static implicit operator University(RegisterDto registrationDto)
        {
            return new University
            {
                Code = registrationDto.UniversityCode,
                Name = registrationDto.UniversityName,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now
            };
        }
    }
}
