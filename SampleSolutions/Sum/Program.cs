using System.Linq;
public class Program {
    static void Main(string[] args)
    {
        System.Console.WriteLine(System.Console.ReadLine().Split(' ').Select(int.Parse).Sum());
    }
}