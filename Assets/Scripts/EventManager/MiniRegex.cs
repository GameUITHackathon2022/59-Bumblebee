namespace EventManager {
    
using System;
    public static class MiniRegex
    {
        private const char any = '?';
        private const char wild = '*';

        private const char bracketStart = '[';
        private const char bracketEnd = ']';
        private const char hyphen = '-';

        private const int numeric09 = 1;
        private const int numeric19 = 2;
        private const int upper = 4;
        private const int lower = 8;

        public static bool IsMatch(string input, string pattern)
        {
            if (pattern == input) return true;
            if (pattern == string.Empty) return false;
            if (input == string.Empty) return IsTailAllWild(pattern, 0);
            if (IsTailAllWild(pattern, 0)) return true;

            var patternIndex = 0;
            var inputIndex = 0;
            var filter = 0;
            
            while (patternIndex < pattern.Length)
            {
                if (patternIndex + 4 < pattern.Length && pattern[patternIndex + 2] == hyphen)
                {
                    if (filter != 0 || pattern[patternIndex] == bracketStart)
                    {
                        if (patternIndex + 5 >= pattern.Length)
                            throw new IndexOutOfRangeException();
                        if (pattern[patternIndex + 1] == '0' && pattern[patternIndex + 3] == '9')
                            filter |= numeric09;
                        else if (pattern[patternIndex + 1] == '1' && pattern[patternIndex + 3] == '9')
                            filter |= numeric19;
                        else if (pattern[patternIndex + 1] == 'A' && pattern[patternIndex + 3] == 'Z')
                            filter |= upper;
                        else if (pattern[patternIndex + 1] == 'a' && pattern[patternIndex + 3] == 'z')
                            filter |= lower;
                        else
                            throw new NotImplementedException();

                        if (pattern[patternIndex + 4] == bracketEnd)
                            patternIndex += 5;
                        else if (patternIndex + 7 < pattern.Length && pattern[patternIndex + 5] == hyphen)
                            patternIndex += 3;
                        else
                            throw new NotImplementedException();

                        continue;
                    }
                }

                if (pattern[patternIndex] == input[inputIndex]
                || pattern[patternIndex] == any)
                {
                    if (filter != 0 && !Filter(input[inputIndex], filter))
                        return false;

                    ++patternIndex;
                    ++inputIndex;

                    if (inputIndex == input.Length)
                        return IsTailAllWild(pattern, patternIndex);

                    filter = 0;
                    continue;
                }

                if (pattern[patternIndex] == wild)
                {
                    if (IsTailAllWild(pattern, patternIndex))
                    {
                        if (filter == 0)
                            return true;

                        for (var i = inputIndex; i < input.Length; ++i)
                        {
                            if (!Filter(input[i], filter))
                                return false;
                        }
                        return true;
                    }

                    if (patternIndex < pattern.Length - 1)
                    {
                        var shifted = 0;

                        while (inputIndex + shifted < input.Length)
                        {
                            if (filter != 0 && !Filter(input[inputIndex + shifted], filter))
                                return false;

                            if (pattern[patternIndex + 1] == input[inputIndex + shifted]
                                || pattern[patternIndex + 1] == any)
                            {
                                ++shifted;

                                if (inputIndex + shifted == input.Length)
                                    return true;

                                if (pattern[patternIndex + 1] != input[inputIndex + shifted]
                                    && pattern[patternIndex + 1] != any)
                                {
                                    ++patternIndex;
                                    inputIndex += shifted - 1;
                                    break;
                                }
                                if (inputIndex + shifted < input.Length)
                                    continue;
                            }

                            ++shifted;

                            if (inputIndex + shifted == input.Length)
                                return false;
                        }

                        filter = 0;
                        continue;
                    }
                }

                return false;
            }

            return false;
        }

        private static bool IsTailAllWild(string format, int startIndex)
        {
            for (var i = startIndex; i < format.Length; ++i)
            {
                if (format[i] != wild)
                    return false;
            }
            return true;
        }

        private static bool Filter(char input, int filter)
        {
            var result = (filter & numeric09) > 0 
                && (input >= '0' && input <= '9');
            if ((filter & numeric19) > 0)
                result = result || (input >= '1' && input <= '9');
            if ((filter & upper) > 0)
                result = result || (input >= 'A' && input <= 'Z');
            if ((filter & lower) > 0)
                result = result || (input >= 'a' && input <= 'z');

            return result;
        }
    }
}