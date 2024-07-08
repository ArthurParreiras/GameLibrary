using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

class Game
{
    public string Nome { get; set; }
    public double Valor { get; set; }
    public string Categoria { get; set; }

    public Game(string nome, double valor, string categoria)
    {
        Nome = nome;
        Valor = valor;
        Categoria = categoria;
    }
}

class User
{
    public string Username { get; set; }
    public string Password { get; set; }

    public User(string username, string password)
    {
        Username = username;
        Password = password;
    }
}

class GameLibrary
{
    private List<User> users = new List<User>();
    private Dictionary<string, Game> games = new Dictionary<string, Game>();  // Tabela Hash
    private List<Game> rentedGames = new List<Game>();

    private string usersFilePath = "users.txt";
    private string gamesFilePath = "games.txt";

    public GameLibrary()
    {
        LoadUsers();
        LoadGames();
    }

    public void RegisterUser(string username, string password)
    {
        users.Add(new User(username, password));
        SaveUsers();
    }

    public bool ValidateLogin(string username, string password)
    {
        return users.Exists(u => u.Username == username && u.Password == password);
    }

    public void ShowMenu(string username)
    {
        Console.WriteLine($"Welcome, {username}!");

        while (true)
        {
            Console.WriteLine("\nMenu:");
            Console.WriteLine("1. Adicionar novo jogo");
            Console.WriteLine("2. Remover jogo");
            Console.WriteLine("3. Listar jogos");
            Console.WriteLine("4. Alugar jogo");
            Console.WriteLine("5. Listar jogos alugados");
            Console.WriteLine("6. Deslogar");

            int choice = GetChoice(1, 6);

            switch (choice)
            {
                case 1:
                    AddNewGame();
                    break;
                case 2:
                    RemoveGame();
                    break;
                case 3:
                    ListGames();
                    break;
                case 4:
                    RentGame();
                    break;
                case 5:
                    ListRentedGames();
                    break;
                case 6:
                    return;
            }
        }
    }

    public int GetChoice(int min, int max)
    {
        int choice;
        while (true)
        {
            Console.Write("Digite a opção: ");
            if (int.TryParse(Console.ReadLine(), out choice) && choice >= min && choice <= max)
            {
                return choice;
            }
            Console.WriteLine("Opção inválida. Tente novamente.");
        }
    }

    private void AddNewGame()
    {
        Console.Write("Insira o nome do jogo: ");
        string name = Console.ReadLine();

        Console.Write("Insira a categoria do jogo: ");
        string category = Console.ReadLine();

        double price = GetDoubleInput("Insira o valor do jogo: ");

        if (!games.ContainsKey(name))
        {
            games[name] = new Game(name, price, category);
            SaveGames();
            Console.WriteLine("Jogo adicionado com sucesso.");
        }
        else
        {
            Console.WriteLine("Já existe um jogo com esse nome. Escolha outro nome.");
        }
    }

    private void RemoveGame()
    {
        Console.Write("Insira o nome do jogo: ");
        string nameToRemove = Console.ReadLine();

        if (games.ContainsKey(nameToRemove))
        {
            games.Remove(nameToRemove);
            SaveGames();
            Console.WriteLine("Jogo removido com sucesso.");
        }
        else
        {
            Console.WriteLine("Jogo não encontrado.");
        }
    }

    private void ListGames()
    {
        Console.WriteLine("Lista de jogos:");

        foreach (var game in games.Values)
        {
            Console.WriteLine($"{game.Nome} - {game.Categoria} - ${game.Valor}");
        }
    }

    private void RentGame()
    {
        Console.Write("Insira o nome do jogo: ");
        string nameToRent = Console.ReadLine();

        if (games.TryGetValue(nameToRent, out var gameToRent))
        {
            rentedGames.Add(gameToRent);
            Console.WriteLine("Jogo alugado com sucesso.");
        }
        else
        {
            Console.WriteLine("Jogo não encontrado.");
        }
    }

    private void ListRentedGames()
    {
        Console.WriteLine("Lista de jogos alugados:");

        foreach (var game in rentedGames)
        {
            Console.WriteLine($"{game.Nome} - {game.Categoria} - ${game.Valor}");
        }
    }

    private double GetDoubleInput(string message)
    {
        double result;
        while (true)
        {
            Console.Write(message);
            if (double.TryParse(Console.ReadLine(), out result))
            {
                return result;
            }
            Console.WriteLine("Valor inválido. Por favor insira um valor válido");
        }
    }

    private void SaveUsers()
    {
        using (StreamWriter writer = new StreamWriter(usersFilePath))
        {
            foreach (var user in users)
            {
                writer.WriteLine($"{user.Username},{user.Password}");
            }
        }
    }

    private void LoadUsers()
    {
        if (File.Exists(usersFilePath))
        {
            users = File.ReadAllLines(usersFilePath)
                        .Select(line => line.Split(','))
                        .Select(parts => new User(parts[0], parts[1]))
                        .ToList();
        }
    }

    private void SaveGames()
    {
        using (StreamWriter writer = new StreamWriter(gamesFilePath))
        {
            foreach (var game in games.Values)
            {
                writer.WriteLine($"{game.Nome},{game.Valor},{game.Categoria}");
            }
        }
    }

    private void LoadGames()
    {
        if (File.Exists(gamesFilePath))
        {
            games = File.ReadAllLines(gamesFilePath)
                        .Select(line => line.Split(','))
                        .ToDictionary(parts => parts[0], parts => new Game(parts[0], double.Parse(parts[1]), parts[2]));
        }
    }
}

class Program
{
    static void Main()
    {
        GameLibrary gameLibrary = new GameLibrary();

        Console.WriteLine("ACERVO DE JOGOS");

        while (true)
        {
            Console.WriteLine("\n1. Cadastrar\n2. Logar\n3. Sair");

            int choice = gameLibrary.GetChoice(1, 3);

            switch (choice)
            {
                case 1:
                    Console.Write("Insira seu usuário: ");
                    string newUsername = Console.ReadLine();

                    Console.Write("Insira sua senha: ");
                    string newPassword = Console.ReadLine();

                    gameLibrary.RegisterUser(newUsername, newPassword);
                    Console.WriteLine("Cadastrado com sucesso.");
                    break;
                case 2:
                    Console.Write("Insira seu usuário: ");
                    string username = Console.ReadLine();

                    Console.Write("Insira sua senha: ");
                    string password = Console.ReadLine();

                    if (gameLibrary.ValidateLogin(username, password))
                    {
                        gameLibrary.ShowMenu(username);
                    }
                    else
                    {
                        Console.WriteLine("O usuário e a senha não correspondem.");
                    }
                    break;
                case 3:
                    Console.WriteLine("Encerrando o programa...");
                    return;
            }
        }
    }
}
