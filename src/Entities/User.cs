using PetProject.Models;
using System;
using System.Xml;

namespace PetProject
{
    public class User
    {
        public Guid UserId {set; get;}
	    public string FullName {set; get;}
        public DateTime? DateOfBirth {set; get;}
	    public Gender? Gender {set; get;}
	    public string Email {set; get;}
        public string PhoneNumber {set; get;}
        public string Password {set; get;}
	    public string Role {set; get;}
	    public bool? IsBlocked {set; get;}
	    public int? WrongPasswordCount {set; get;}
        public DateTime CreatedDate {set; get;}
    }
}
