using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INFL.Data
{
    public class CodeGenerator
    {
        private static readonly char[] Characters = "0123456789abcdefghijklmnopqrstuvwxyz".ToCharArray();

        public static string GenerateNextCode(string lastCode)
        {
            // تحويل الكود الأخير إلى مصفوفة من الخانات (مثل "001" إلى {'0', '0', '1'})
            var codeArray = lastCode.ToCharArray();

            // ابدأ من آخر خانة وقم بتحديث الكود
            for (int i = codeArray.Length - 1; i >= 0; i--)
            {
                int currentIndex = Array.IndexOf(Characters, codeArray[i]);

                // إذا كانت الخانة الأخيرة هي آخر قيمة، قم بإعادة ضبطها إلى أول عنصر
                if (currentIndex == Characters.Length - 1)
                {
                    codeArray[i] = Characters[0];
                }
                else
                {
                    // زيادة الخانة التالية وتوقف
                    codeArray[i] = Characters[currentIndex + 1];
                    break;
                }
            }

            return new string(codeArray);

        }
    }
}
