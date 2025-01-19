using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace PL.Volunteer
{
    public class DigitValidation : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string? phoneNumber = value.ToString();
            if (!string.IsNullOrEmpty(phoneNumber))
            {
                if (phoneNumber.Length == 10 && phoneNumber[0] == '0' && phoneNumber[1] == '5')
                    return ValidationResult.ValidResult; // תקין
            }

            return new ValidationResult(false, "יש להזין מספר בעל 10 ספרות שמתחיל ב05.");
        }
    }

    public class EmailValidation : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string? email = value.ToString();
            if (!string.IsNullOrEmpty(email))
            {
                if (email.Contains("@gmail.com") == true || email.Contains("@walla.co.il") == true || email.Contains("@g.jct.ac.il") == true)

                    if (email[0] != '.' && email[email.Length - 1] != '.')

                        if (email.Contains("..") == false && email.Contains(" ") == false)
                            return ValidationResult.ValidResult; // תקין
            }

            return new ValidationResult(false, "כתובת האימייל אינה חוקית");
        }
    }

    public class PassValidation : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string? password = value.ToString();
            if (!string.IsNullOrEmpty(password))
            {
                if (password.Length == 8)
                    return ValidationResult.ValidResult; // תקין
            }

            return new ValidationResult(false, "סיסמא אינה חוקית");
        }
    }

    public class DisValidation : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value == null)
                return ValidationResult.ValidResult;

            double distance = (double)(value);
            if (distance >= 0)
                return ValidationResult.ValidResult; // תקין

            return new ValidationResult(false, "מרחק לא יכול להיות שלילי");
        }
    }

    public class IDValidation : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value != null)
            {
                int Id = (int)value;
                if (Id < 10000000 || Id > 999999999)
                {
                    string id = value.ToString()!;
                    if (id.Any(char.IsUpper) == false && id.Any(char.IsLower) == false)
                        return ValidationResult.ValidResult; // תקין
                }
            }
            return new ValidationResult(false, "סיסמא אינה חוקית");
        }
    }



}

    