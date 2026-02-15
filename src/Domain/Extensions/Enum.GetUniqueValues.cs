namespace ReSR.Domain.Extensions;
internal static partial class FlagsManipulation {

    internal static IEnumerable<F> GetUniqueValues<F>() where F: Enum {

        ulong flag = 1;
        foreach (F value in Enum.GetValues(typeof(F))) {
            ulong bits = Convert.ToUInt64(value);
            while (flag < bits)
                flag <<= 1;

            if (flag == bits)
                yield return value;

        }
    }
    

    public static IEnumerable<F> GetUniqueValues<F>(this F flags) where F: Enum {

        ulong flag = 1;
        foreach (F value in Enum.GetValues(typeof(F))) {

            ulong bits = Convert.ToUInt64(value);
            while (flag < bits)
                flag <<= 1;

            if (flag == bits && flags.HasFlag(value))
                yield return value;

        }
    }
}