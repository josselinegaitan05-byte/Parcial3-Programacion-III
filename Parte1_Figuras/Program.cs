// Parcial #3 - Parte 1: Jerarquía de clases con clase abstracta y polimorfismo
using System;

namespace Parte1_Figuras
{
    // 1. CLASE ABSTRACTA: actúa como molde obligatorio para todas las figuras.
    //    No se puede instanciar directamente (new Figura() da error).
    public abstract class Figura
    {
        // Métodos abstractos: no tienen cuerpo, cada clase hija DEBE implementarlos.
        public abstract double CalcularArea();
        public abstract double CalcularPerimetro();

        // Método concreto común a todas las figuras.
        public string ObtenerNombre() => GetType().Name;
    }

    // 2. CLASES DERIVADAS: desarrollan (override) los métodos abstractos.
    public class Rectangulo : Figura
    {
        public double Base { get; }
        public double Altura { get; }

        public Rectangulo(double baseRect, double altura)
        {
            Base = baseRect;
            Altura = altura;
        }

        public override double CalcularArea() => Base * Altura;
        public override double CalcularPerimetro() => 2 * (Base + Altura);
    }

    public class Cuadrado : Figura
    {
        public double Lado { get; }

        public Cuadrado(double lado)
        {
            Lado = lado;
        }

        public override double CalcularArea() => Lado * Lado;
        public override double CalcularPerimetro() => 4 * Lado;
    }

    public class Triangulo : Figura
    {
        public double LadoA { get; }
        public double LadoB { get; }
        public double LadoC { get; }

        public Triangulo(double ladoA, double ladoB, double ladoC)
        {
            // Validación de la desigualdad triangular
            if (ladoA + ladoB <= ladoC || ladoA + ladoC <= ladoB || ladoB + ladoC <= ladoA)
                throw new ArgumentException("Los lados no forman un triángulo válido.");

            LadoA = ladoA;
            LadoB = ladoB;
            LadoC = ladoC;
        }

        // Área con la fórmula de Herón
        public override double CalcularArea()
        {
            double s = CalcularPerimetro() / 2;
            return Math.Sqrt(s * (s - LadoA) * (s - LadoB) * (s - LadoC));
        }

        public override double CalcularPerimetro() => LadoA + LadoB + LadoC;
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Parcial #3 - Parte 1: Figuras y Polimorfismo ===\n");

            // 3. ARREGLO UNIDIMENSIONAL (vector) DE TIPO PADRE: Figura[]
            //    Almacena instancias de distintas clases hijas.
            Figura[] figuras = new Figura[]
            {
                new Rectangulo(5, 3),
                new Cuadrado(4),
                new Triangulo(3, 4, 5),
                new Rectangulo(10, 2.5),
                new Cuadrado(7.5),
                new Triangulo(6, 6, 6)
            };

            Console.WriteLine($"{"#",-3}{"Figura",-14}{"Área",12}{"Perímetro",14}");
            Console.WriteLine(new string('-', 43));

            // 4. RECORRIDO POLIMÓRFICO: cada elemento se trata como Figura,
            //    pero en tiempo de ejecución se invoca el método de la clase hija real.
            for (int i = 0; i < figuras.Length; i++)
            {
                Figura f = figuras[i];
                Console.WriteLine($"{i + 1,-3}{f.ObtenerNombre(),-14}{f.CalcularArea(),12:F2}{f.CalcularPerimetro(),14:F2}");
            }

            Console.WriteLine("\nTotal de figuras procesadas: " + figuras.Length);
        }
    }
}
