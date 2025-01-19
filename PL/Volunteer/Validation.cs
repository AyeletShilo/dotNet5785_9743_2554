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
            string phoneNumber= value.ToString();
            if (phoneNumber.Length == 10 && phoneNumber[0] == '0' && phoneNumber[1] == '5')
                return ValidationResult.ValidResult; // תקין
                
            return new ValidationResult(false, "יש להזין מספר בעל 10 ספרות שמתחיל ב05.");
        }
    }

    public class Validation : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            // בדיקה אם הערך שהוזן הוא מספר שלם
            if (int.TryParse(value as string, out int number))
            {
                // בדיקה אם המספר חיובי
                if (number >= 0)
                {
                    return ValidationResult.ValidResult; // תקין
                }
            }

            // אם הנתון לא תקין, מחזירים הודעת שגיאה
            return new ValidationResult(false, "יש להזין מספר חיובי בלבד.");
        }
    }
}
