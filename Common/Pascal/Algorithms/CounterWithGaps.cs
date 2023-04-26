using Pascal;
using static Pascal.TestEngine;


public static class CounterWithGaps
{
    public static void Tests()
    {
        OriginalCounter(6, 6);
        BetterCounter(6, 6);
    }

    public static void OriginalCounter(int maxOn, int maxOff)
    {
        Test($"OriginalCounter counts 10 bits with never more than {maxOn} set to 1 or {maxOff} set to 0", () =>
        {

            int res = 0;
            // counting 0 to 255 without any more than 5 bits set to 0 or 5 bits set to 1
            int ifs = 0;

            for (int i0 = 0; i0 <= 1; i0++)
            {
                for (int i1 = 0; i1 <= 1; i1++)
                {
                    for (int i2 = 0; i2 <= 1; i2++)
                    {
                        for (int i3 = 0; i3 <= 1; i3++)
                        {
                            for (int i4 = 0; i4 <= 1; i4++)
                            {
                                for (int i5 = 0; i5 <= 1; i5++)
                                {
                                    ifs += 1;
                                    int s0to5 = i0 + i1 + i2 + i3 + i4 + i5;
                                    if (s0to5 > maxOn || 6 - s0to5 > maxOff) continue;

                                    for (int i6 = 0; i6 <= 1; i6++)
                                    {
                                        ifs += 1;
                                        int s0to6 = s0to5 + i6;
                                        if (s0to6 > maxOn || 7 - s0to6 > maxOff) continue;
                                        for (int i7 = 0; i7 <= 1; i7++)
                                        {
                                            ifs += 1;
                                            int s0to7 = s0to6 + i7;
                                            if (s0to7 > maxOn || 8 - s0to7 > maxOff) continue;
                                            for (int i8 = 0; i8 <= 1; i8++)
                                            {
                                                ifs += 1;
                                                int s0to8 = s0to7 + i8;
                                                if (s0to8 > maxOn || 9 - s0to8 > maxOff) continue;
                                                for (int i9 = 0; i9 <= 1; i9++)
                                                {
                                                    ifs += 1;
                                                    int s0to9 = s0to8 + i9;
                                                    if (s0to9 > maxOn || 10 - s0to9 > maxOff) continue;
                                                    res += 1;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            Console.WriteLine($"Result: {res} with {ifs} ifs");
        });
    }

    public static void BetterCounter(int maxOn, int maxOff)
    {
        Test($"BetterCounter counts 10 bits with never more than {maxOn} set to 1 or {maxOff} set to 0", () =>
        {

            int res = 0;
            // counting 0 to 255 without any more than 5 bits set to 0 or 5 bits set to 1
            int ifs = 0;

            //List<Tuple<int, int, int, int, int>>[] lists = new [](6, () => new List<Tuple<int, int, int, int, int>>());
            int[] byNbOns = new int[6];

            for (int i0 = 0; i0 <= 1; i0++)
            {
                for (int i1 = 0; i1 <= 1; i1++)
                {
                    for (int i2 = 0; i2 <= 1; i2++)
                    {
                        for (int i3 = 0; i3 <= 1; i3++)
                        {
                            for (int i4 = 0; i4 <= 1; i4++)
                            {
                                int nbOn = i0 + i1 + i2 + i3 + i4;
                                int nbOff = 5 - nbOn;
                                byNbOns[nbOn] += 1;
                            }
                        }
                    }
                }
            }
            for (int onLeft = 0; onLeft <= 5; onLeft++)
            {
                for (int onRight = 0; onRight <= 5; onRight++)
                {
                    ifs += 1;
                    int nbOn = onLeft + onRight;
                    int nbOff = 10 - nbOn;
                    if (nbOn > maxOn || nbOff > maxOff) continue;
                    res += byNbOns[onLeft] * byNbOns[onRight];
                }
            }
            Console.WriteLine($"Result: {res} with {ifs} ifs");
        });
    }
}