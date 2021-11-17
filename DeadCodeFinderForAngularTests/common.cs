using System;
using System.Net.Mail;
using AutoFixture;


namespace DeadCodeFinderForAngularTests
{
    public class common
    {
    }
    public static class RandomGenerator
    {
        public static int RandomInt => new Fixture().Create<int>();
        public static int RandomRangedInt(int min, int max) => new Random().Next(min, max + 1);
        public static bool RandomBool => new Fixture().Create<bool>();
        public static string RandomString => new Fixture().Create<string>();
        public static string RandomEmailAddress => new Fixture().Create<MailAddress>().Address;
        public static DateTime RandomDate => new Fixture().Create<DateTime>();
        public static Guid RandomGuid => new Fixture().Create<Guid>();

        public static DateTime RandomRangedDate(DateTime start, DateTime end)
        {
            return start.AddDays(RandomRangedInt(0, (end - start).Days));
        }

        public static object RandomAmong(params object[] values)
        {
            if (values.Length < 1) return null;
            if (values.Length == 1) return values[0].ToString();

            return values[RandomRangedInt(0, values.Length - 1)];
        }

        public static byte[] RandomBytes(int count)
        {
            Random rand = new Random();
            Byte[] bytes = new Byte[count];
            rand.NextBytes(bytes);
            return bytes;
        }

    }
}
