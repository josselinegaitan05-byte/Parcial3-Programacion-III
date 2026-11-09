// Parcial #3 - Parte 2: Trabajar con JSON usando Newtonsoft.Json
// Paso 1: Instalar  ->  dotnet add package Newtonsoft.Json
// Paso 2: Importar
using Newtonsoft.Json;

Console.WriteLine("Hello, World!");

// Paso 3: Crear objeto con datos
var persona = new
{
    Nombre = "Cristell",
    Edad = 20,
    Carrera = "Ingenieria en Sistemas"
};

// Paso 4: Convertir objeto a cadena JSON
string json = JsonConvert.SerializeObject(persona);

Console.WriteLine(json);
