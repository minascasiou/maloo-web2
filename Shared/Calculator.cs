using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//namespace MALOO
namespace BlazorApp.Shared
{
    public static class Calculator
    {
        public static string outputText = string.Empty;
        public static int slotLength = 5400;
        public static List<int> rodLengths = new List<int>
        {
            //13270, 16270, 18950, 8900, 16450, 11880, 11360, 10970
            4150 ,2884 ,1008 ,2305 ,2884 ,200 ,3630 ,920 ,2290 ,425 ,3645 ,4325 ,805 ,3560 ,2650 ,294 ,294 ,294 ,294 ,261 ,261 ,5230 ,914 ,350 ,1568 ,161 ,5755 ,3945 ,3042 ,130 ,180 ,2687 ,146 ,3985 ,3985 ,3985 ,3435 ,3971 ,3961 ,3985 ,3435 ,3975, 5564, 1516
        };          //Note: last measurement - 8900, is for the stairs

        public static void Calculate(bool addSlotOnExceedingLength, bool sumOverSizeLengths = false)
        {

            WriteLine("++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++");
            WriteLine($"MALOO: addSlotOnExceedingLength={addSlotOnExceedingLength} sumOverSizeLengths={sumOverSizeLengths}");
            WriteLine("--------------------------------------------------------------------------------------------------");



            var (optimizedSlots, overSizeInfo) = OptimizeSlotAllocation(slotLength, rodLengths, addSlotOnExceedingLength, sumOverSizeLengths);

            WriteLine("Optimized Slot Allocation:");

            WriteLine("\nStandard Slots:");
            for (int i = 0; i < optimizedSlots.Count; i++)
            {
                WriteLine($"{(i + 1).ToString().PadLeft(2)} Slot Length: {optimizedSlots[i].Sum()} \t Rods: {string.Join(", ", optimizedSlots[i])}");
            }

            if (addSlotOnExceedingLength)
            {
                WriteLine("\nOversize Rods:");
                foreach (var rod in overSizeInfo.OversizedRods)
                {
                    WriteLine($"Rod Length: {rod}");
                }

                if (sumOverSizeLengths)
                {
                    WriteLine("\nCombined Oversize Lengths in Slots:");
                    int remainingLength = overSizeInfo.TotalLength;
                    for (int i = 0; i < overSizeInfo.AdditionalSlots; i++)
                    {
                        int slotFill = Math.Min(slotLength, remainingLength);
                        WriteLine($"Slot {i + 1}: {slotFill}");
                        remainingLength -= slotFill;
                    }
                }
                else
                {
                    WriteLine("\nOversize Rods in Individual Slots:");
                    foreach (var rod in overSizeInfo.OversizedRods)
                    {
                        int slotsNeeded = (int)Math.Ceiling((double)rod / slotLength);
                        WriteLine($"Rod Length: {rod}");
                        for (int j = 0; j < slotsNeeded; j++)
                        {
                            int slotFill = Math.Min(slotLength, rod - j * slotLength);
                            WriteLine($"  Slot {j + 1}: {slotFill}");
                        }
                        WriteLine("");
                    }
                }

                WriteLine($"\nTotal number of standard slots: {optimizedSlots.Count}");
                WriteLine($"Number of oversized rods: {overSizeInfo.OversizedRodCount}");
                WriteLine($"Total length of oversized rods: {overSizeInfo.TotalLength}");
                WriteLine($"Additional slots required for oversized rods: {overSizeInfo.AdditionalSlots}");
                WriteLine($"Total number of slots required: {optimizedSlots.Count + overSizeInfo.AdditionalSlots}");
            }
            else if (overSizeInfo.OversizedRodCount > 0)
            {
                WriteLine("\nOmitted rods (exceeding slot length):");
                foreach (var rod in overSizeInfo.OversizedRods)
                {
                    WriteLine($"Rod Length: {rod}");
                }
            }
        }

        static (List<List<int>> slots, (int TotalLength, int AdditionalSlots, int OversizedRodCount, List<int> OversizedRods)) OptimizeSlotAllocation(
        int slotLength, List<int> rodLengths, bool addSlotOnExceedingLength, bool sumOverSizeLengths)
        {
            List<List<int>> slots = new List<List<int>>();
            List<int> oversizedRods = new List<int>();
            int totalOversizedLength = 0;
            rodLengths.Sort((a, b) => b.CompareTo(a)); // Sort in descending order

            for (int i = 0; i < rodLengths.Count; i++)
            {
                if (rodLengths[i] > slotLength)
                {
                    oversizedRods.Add(rodLengths[i]);
                    totalOversizedLength += rodLengths[i];
                    rodLengths.RemoveAt(i);
                    i--;
                    continue;
                }

                List<int> currentSlot = new List<int> { rodLengths[i] };
                int remainingSpace = slotLength - rodLengths[i];

                for (int j = i + 1; j < rodLengths.Count; j++)
                {
                    if (rodLengths[j] <= remainingSpace)
                    {
                        currentSlot.Add(rodLengths[j]);
                        remainingSpace -= rodLengths[j];
                        rodLengths.RemoveAt(j);
                        j--;
                    }
                }

                slots.Add(currentSlot);
                rodLengths.RemoveAt(i);
                i--;
            }

            int additionalSlots = 0;
            if (addSlotOnExceedingLength)
            {
                additionalSlots = (int)Math.Ceiling((double)totalOversizedLength / slotLength);
            }

            var overSizeInfo = (
                TotalLength: totalOversizedLength,
                AdditionalSlots: additionalSlots,
                OversizedRodCount: oversizedRods.Count,
                OversizedRods: oversizedRods
            );

            return (slots, overSizeInfo);
        }

        private static void WriteLine(string line)
        {
            outputText += line + "<br/>";
            //outputText += line + Environment.NewLine;
        }
    }
}
