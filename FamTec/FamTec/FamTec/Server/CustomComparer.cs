using System.Text.RegularExpressions;

namespace FamTec.Server
{
    public class CustomComparer : IComparer<string>
    {
        public int Compare(string x, string y)
        {
            try
            {
                // 정규식을 이용해 숫자와 문자로 분리
                var xParts = SplitStringAndNumbers(x);
                var yParts = SplitStringAndNumbers(y);

                int minLength = Math.Min(xParts.Count, yParts.Count);

                for (int i = 0; i < minLength; i++)
                {
                    int result;

                    // 둘 다 숫자인 경우 숫자로 비교
                    if (int.TryParse(xParts[i], out int xNum) && int.TryParse(yParts[i], out int yNum))
                    {
                        result = xNum.CompareTo(yNum);
                    }
                    // 둘 다 문자인 경우 문자로 비교
                    else if (!int.TryParse(xParts[i], out _) && !int.TryParse(yParts[i], out _))
                    {
                        result = string.Compare(xParts[i], yParts[i], StringComparison.Ordinal);
                    }
                    // 하나는 숫자이고 하나는 문자인 경우 숫자가 먼저 오도록 정렬
                    else
                    {
                        result = int.TryParse(xParts[i], out _) ? -1 : 1;
                    }

                    if (result != 0) return result; // 비교 결과가 0이 아니면 결과 반환
                }

                // 길이가 다른 경우 더 긴 쪽이 뒤로 오도록 정렬
                return xParts.Count.CompareTo(yParts.Count);
            }
            catch(Exception ex)
            {
                throw;
            }
        }

        // 문자열을 문자와 숫자로 분리하는 함수
        private List<string> SplitStringAndNumbers(string input)
        {
            return Regex.Split(input, @"(\d+)").Where(s => s.Length > 0).ToList();
        }
    }
}
