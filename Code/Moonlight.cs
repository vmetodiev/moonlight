using System;
using System.Data;
using System.Data.SqlClient;

namespace moonlight
{
	class Moonlight
	{
		// User not privileged to execute SQL without saying the magic word
		private static bool privEnabled = false;

		// MAIN
		[STAThread]
		static void Main(string[] args)
		{
			// Initialise the connection strings array
			for (int index = 0; index < SQLOperator.connectionStrings.Length; index++)
				SQLOperator.connectionStrings[index] = string.Empty;

			SQLOperator.connectionStrings[0] = "Server=127.0.0.1;Database=super;User Id=sa;Password=sa; Integrated Security=true";   // 1st 
			SQLOperator.connectionStrings[1] = "Server=127.0.0.1;Database=ultra;User Id=sa;Password=sa; Integrated Security=true";   // 2nd
			SQLOperator.connectionStrings[2] = "Server=127.0.0.1;Database=master;User Id=sa;Password=sa; Integrated Security=true";  // 3rd

			// Give us the chance to say the magic word
			string magicWordLine = Menu.ReadMagicWord();
			privEnabled = Menu.EscalatePrivilege(magicWordLine);

			// Stop silently if case of wrong magic word
			if (!privEnabled)
			{
				Menu.promptType = Menu.PromptType.NonPriv;
				return;
			}
			else
				Menu.promptType = Menu.PromptType.Priv;

			// User "shell" input
			Menu.ReadInput();
		}
	}

	class SQLOperator
	{
		private const string SqlEnd = "end";
		public const int connectionStringsCount = 10;
		public static string[] connectionStrings = new string[connectionStringsCount];

		public static int GetLastActualConnectionStringIndex()
		{
			for (int index = 0; index < connectionStrings.Length; index++)
				if (connectionStrings[index].Equals(string.Empty))
					return index;

			return -1;
		}

		// Can also be used as a simple for connection string connectivity
		public static void SqlShowTables(int connStringId)
		{
			// Create and open the connection in a using block. This
			// ensures that all resources will be closed and disposed
			// when the code exits.
			using (SqlConnection connection =
					   new SqlConnection(connectionStrings[connStringId]))
			{
				// Show tables
				string queryString = "SELECT * FROM information_schema.tables;";

				// Create the Command
				SqlCommand command = new SqlCommand(queryString, connection);

				// Open the connection in a try/catch block.
				// Create and execute the DataReader, writing the result
				// set to the console window.
				try
				{
					connection.Open();

					SqlDataReader reader = command.ExecuteReader();
					int fieldCount = reader.FieldCount;

					while (reader.Read())
					{
						Console.Write("\n|");
						for (int index = 0; index < fieldCount; index++)
							Console.Write("{0}|", reader[index]);
					}
					Console.WriteLine("\n");
					reader.Close();
				}
				catch (Exception ex)
				{
					Console.WriteLine("[-] {0}", ex.Message);
					Console.WriteLine("[-] Could not connect...");
				}
			}
		}

		public static void EnterConnection(int connStringId)
		{
			// Create and open the connection in a using block. This
			// ensures that all resources will be closed and disposed
			// when the code exits.
			using (SqlConnection connection =
					   new SqlConnection(connectionStrings[connStringId]))
			{

				string inputLine = string.Empty;

				// Open the connection in a try/catch block.
				// Create and execute the DataReader, writing the result
				// set to the console window.
				try
				{
					connection.Open();
					Console.WriteLine("[+] Entering connection...");
					Menu.promptType = Menu.PromptType.Sql; // Changing the prompt to SQL

					// // //
					while (inputLine != SqlEnd)
					{
						try
						{
							Menu.ShowPrompt(Menu.promptType);
							inputLine = Console.ReadLine();

							if (Menu.IsValidationEnabled())
							{
								bool validInput = Menu.ValidateInput(inputLine);
								if (!validInput)
									continue;
							}

							SqlCommand command = new SqlCommand(inputLine, connection);
							SqlDataReader reader = command.ExecuteReader();
							int fieldCount = reader.FieldCount;

							while (reader.Read())
							{
								Console.Write("\n|");
								for (int index = 0; index < fieldCount; index++)
									Console.Write("{0}|", reader[index]);
							}
							Console.WriteLine("\n");
							reader.Close();
						}
						catch (Exception ex)
						{
							Console.WriteLine("[-] {0}", ex.Message);
							Console.WriteLine("[-] SQL execution failed...");
						}
					}
					// // //
					connection.Close();
					Console.WriteLine("[+] Leaving connection...");
					Menu.promptType = Menu.PromptType.Priv; // Setting prompt back

				}
				catch (Exception ex)
				{
					Console.WriteLine("[-] {0}", ex.Message);
					Console.WriteLine("[-] Could not connect...");
				}
			}
		}
	}

	class Menu
	{
		private const string ShowHintsCommand = "show hints";
		private const string ShowConnectionsCommand = "show connections";
		private const string TestConnectionCommand = "test connection";
		private const string AddConnectionCommand = "add connection";
		private const string EnterConnectionCommand = "enter connection";
		private const string EnableValidationCommand = "enable validation";
		private const string DisableValidationCommand = "disable validation";
		private const string QuitCommand = "quit";

		private static bool validationEnabled = true;

		public enum PromptType
		{
			NonPriv,
			Priv,
			Sql
		}

		public static PromptType promptType;

		public static void ShowPrompt(PromptType promptType)
		{
			switch (promptType)
			{
				case PromptType.Priv:
					Console.Write("->");
					break;

				case PromptType.Sql:
					Console.Write(">>");
					break;

				case PromptType.NonPriv:
					break;

				default:
					break;
			}

			return;
		}

		public static string ReadMagicWord()
		{
			string magicWordLine = Console.ReadLine();
			return magicWordLine;
		}

		public static void ReadInput()
		{
			string inputLine = String.Empty;

			while (inputLine != QuitCommand)
			{
				ShowPrompt(promptType);
				inputLine = Console.ReadLine();

				// Menu selections and options
				string command = GetCommandFromInputLine(inputLine);
				switch (command)
				{
					// Show Hints
					case ShowHintsCommand:
						ShowHints();
						break;

					// Show Connections
					case ShowConnectionsCommand:
						ShowConnectionStrings();
						break;

					// Test Connection
					case TestConnectionCommand:
						try
						{
							int connIndex = ParseDigitFromCommand(inputLine, TestConnectionCommand);
							Console.WriteLine("[+] Executing 'SELECT * FROM information_schema.tables': ");
							SQLOperator.SqlShowTables(connIndex);
						}
						catch (Exception)
						{
							Console.WriteLine("[-] Command execution went wrong, check parameter format and values...");
						}
						break;

					// Enter Connection
					case EnterConnectionCommand:
						try
						{
							int connIndex = ParseDigitFromCommand(inputLine, EnterConnectionCommand);
							SQLOperator.EnterConnection(connIndex);
						}
						catch (Exception)
						{
							Console.WriteLine("[-] Command execution went wrong, check parameter format and values...");
						}
						break;

					// Add connection
					case AddConnectionCommand:
						try
						{
							string newConnectionString = ParseStringFromCommand(inputLine, AddConnectionCommand);
							int connectionStringsArrayIndex = SQLOperator.GetLastActualConnectionStringIndex();

							if (connectionStringsArrayIndex < SQLOperator.connectionStringsCount)
							{
								SQLOperator.connectionStrings[connectionStringsArrayIndex++] = newConnectionString;
								Console.WriteLine("[+] Connection string successfully added...");
							}
							else
								Console.WriteLine("[-] Cannot add more connection strings...");
						}
						catch (Exception)
						{
							Console.WriteLine("[-] Command execution went wrong, check parameter format and values...");
						}
						break;

					// Enable validation
					case EnableValidationCommand:
						Menu.EnableValidation();
						if (Menu.IsValidationEnabled())
							Console.WriteLine("[+] Validation enabled.");
						break;

					// Disable validation
					case DisableValidationCommand:
						Menu.DisableValidation();
						if (!Menu.IsValidationEnabled())
							Console.WriteLine("[+] Validation disabled. Please be careful...");
						break;

					// Default
					default:
						break;
				}
			}
			return;
		}

		public static void EnableValidation()
		{
			validationEnabled = true;
		}

		public static void DisableValidation()
		{
			validationEnabled = false;
		}

		public static bool IsValidationEnabled()
		{
			return validationEnabled;
		}


		private static string GetCommandFromInputLine(string line)
		{
			if (line.IndexOf(ShowHintsCommand) == 0)
				return ShowHintsCommand;
			else if (line.IndexOf(ShowConnectionsCommand) == 0)
				return ShowConnectionsCommand;
			else if (line.IndexOf(TestConnectionCommand) == 0)
				return TestConnectionCommand;
			else if (line.IndexOf(EnterConnectionCommand) == 0)
				return EnterConnectionCommand;
			else if (line.IndexOf(AddConnectionCommand) == 0)
				return AddConnectionCommand;
			else if (line.IndexOf(EnableValidationCommand) == 0)
				return EnableValidationCommand;
			else if (line.IndexOf(DisableValidationCommand) == 0)
				return DisableValidationCommand;
			else
				return string.Empty;

		}

		private static int ParseDigitFromCommand(string line, string command)
		{
			string trimmed = line.Substring(command.Length, line.Length - command.Length);
			string param = string.Empty;

			bool beforeFirstDigit = true;
			for (int i = 0; i < trimmed.Length; i++)
			{
				try
				{
					param += int.Parse(trimmed[i].ToString());
					beforeFirstDigit = false;
				}
				catch (Exception)
				{
					if (!beforeFirstDigit)
						break;
				}
			}
			return int.Parse(param);
		}

		public static string ParseStringFromCommand(string line, string command)
		{
			string trimmed = line.Substring(command.Length, line.Length - command.Length);

			string[] charsToRemove = new string[] { "\"", "'" }; // User may want to enter the string within some quotes...
			foreach (string c in charsToRemove)
			{
				trimmed = trimmed.Replace(c, string.Empty);
			}

			return trimmed;
		}

		public static bool ValidateInput(string line)
		{
			string[] delimited = line.Split(' ');
			string[] illegalStatements = new string[] { "DELETE", "UPDATE", "DROP" }; // A bit dangerous if used by mistake...
			string firstWord = string.Empty;

			foreach (string element in delimited)
			{
				if (element != string.Empty)
				{
					firstWord = element;
					break;
				}

			}

			foreach (string illegalStatement in illegalStatements)
			{
				firstWord = firstWord.ToLower();
				string illegalStatementLower = illegalStatement.ToLower();
				if (firstWord.Equals(illegalStatementLower))
				{
					Console.WriteLine("[-] Query seems to be risky...");
					return false;
				}
			}
			Console.WriteLine("[+] Valid.");
			return true;
		}

		public static bool EscalatePrivilege(string magicWord)
		{
			const string privPassword = "enable";
			if (magicWord.Equals(privPassword))
			{
				ShowMotd();
				return true;
			}
			else
				return false;
		}

		public static void ShowHints()
		{
			Console.WriteLine("Some hints and dirty tricks...");
			string[] hintsStringArray = new string[]
			{
				"Privileged CLI",
				"|",
				"| ->This is the prompt",
				"|",
				"|- show connections",
				"|- test connection [(int)id]",
				"|- add connection [(string)connection_string]",
				"|- enter connection [(int)id]",
				"|- enable validation",
				"|- disable validation",
				"|- quit",
				" ",
				"SQL CLI",
				"|",
				"| >>This is the prompt",
				"|",
				"| - SELECT ... (any SQL syntax)",
				"| - end"
			};

			foreach (string hintString in hintsStringArray)
				Console.WriteLine(hintString);

			return;
		}

		public static void ShowConnectionStrings()
		{
			int i = 0;
			foreach (string connString in SQLOperator.connectionStrings)
			{
				if (!connString.Equals(string.Empty))
					Console.WriteLine("[{0}]: {1}",
						i++,
						connString);
			}
			return;
		}

		public static void ShowMotd()
		{
			Console.WriteLine(
				"\n ---------------------------------------\n"
				+ "|                                       |\n"
				+ "*  MoonLight is about to shine on us... *\n"
				+ "|                                       |\n"
				+ " ---------------------------------------\n"
			);
		}
	}
}