using System.Data;
using SubATE_WPF;
using Microsoft.Data.SqlClient;

namespace Database;

public class Connection
{
    private DataTable _subscriberTable = new();
    private const string ConnectionString = 
        "Server=localhost,1433;" +
        "Database=БазаАбонентов;" +
        "User Id=SA;Password=nqM9Ykigd9e3cJnOpNhkJq54TmedyuoO;" +
        "Encrypt=false;TrustServerCertificate=true;\n";

    public Connection()
    {
        CreateTable();
    }
    
    public static void NewConnection(Action<SqlConnection> dbQuery)
    {
        using (var connection = new SqlConnection(ConnectionString))
        {
            connection.Open();
            try
            {
                dbQuery(connection);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }

    private void CreateTable()
    {
        NewConnection(connection =>
        {
            using (var command = new SqlCommand("CreateDatabase", connection))
            {
                command.ExecuteNonQuery();
            }

            using (var command = new SqlCommand("CreateSubscriberTable", connection))
            {
                command.ExecuteNonQuery();
            }
        });
    }

    public void AddSubscriber(Subscriber subscriber)
    {
        NewConnection(dbQuery: connection =>
        {
            string query = 
                "INSERT INTO Абонент (Имя, Телефон, Премиум, Тип, ДатаРегистрации)\n"+
                "VALUES (@Имя, @Телефон, @Премиум, @Тип, @ДатаРегистрации);";
            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Имя", subscriber.Name);
                command.Parameters.AddWithValue("@Телефон", subscriber.Phone);
                command.Parameters.AddWithValue("@Премиум", subscriber.IsPremium);
                command.Parameters.AddWithValue("@Тип", subscriber.Type.ToString());
                command.Parameters.AddWithValue("@ДатаРегистрации", subscriber.RegistrationDate.ToString());

                command.ExecuteNonQuery();
            }
        });
    }
    
    public Subscriber? GetSubscriber(int id)
    {
        Subscriber? subscriber = null;
        NewConnection(connection =>
        {
            string query = "SELECT * FROM Абонент WHERE ID = @ID";
            using (var adapter = new SqlDataAdapter(query, connection))
            {
                adapter.SelectCommand.Parameters.AddWithValue("@ID", id);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                if (dataTable.Rows.Count > 0)
                {
                    DataRow row = dataTable.Rows[0];
                    subscriber = new Subscriber
                    {
                        Id = Convert.ToInt32(row["Id"]),
                        Name = row["Имя"].ToString() ?? "Нет",
                        Phone = row["Телефон"].ToString() ?? "Нет",
                        IsPremium = Convert.ToBoolean(row["Премиум"]),
                        Type = Enum.TryParse(row["Тип"].ToString(), out SubscriberType type) ? type : SubscriberType.Физическое,
                        RegistrationDate = DateOnly.FromDateTime(Convert.ToDateTime(row["ДатаРегистрации"]))
                    };
                }
            }
        });
        return subscriber;
    }

    public void UpdateSubscriber(Subscriber subscriber)
    {
        NewConnection(connection =>
        {
            string query = 
                "UPDATE Абонент SET Имя = @Имя, Телефон = @Телефон, Премиум = @Премиум, Тип = @Тип, ДатаРегистрации = @ДатаРегистрации\n"+
                "WHERE Id = @Id;";
            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Id", subscriber.Id);
                command.Parameters.AddWithValue("@Имя", subscriber.Name);
                command.Parameters.AddWithValue("@Телефон", subscriber.Phone);
                command.Parameters.AddWithValue("@Премиум", subscriber.IsPremium);
                command.Parameters.AddWithValue("@Тип", subscriber.Type.ToString());
                command.Parameters.AddWithValue("@ДатаРегистрации", subscriber.RegistrationDate.ToString());

                command.ExecuteNonQuery();
            }
        });
    }

    public void DeleteSubscriber(int id)
    {
        NewConnection(connection =>
        {
            string query = "DELETE FROM Абонент WHERE Id = @Id;";
            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Id", id);
                command.ExecuteNonQuery();
            }
        });
    }

    public void LoadSubscribers()
    {
        NewConnection(connection =>
        {
            string query = "SELECT * FROM Абонент";
            using (var adapter = new SqlDataAdapter(query, connection))
            {
                var dataSet = new DataSet();
                adapter.Fill(dataSet, "Абонент");
                _subscriberTable = dataSet.Tables["Абонент"]!;
            }
        });
    }
    
    public List<Subscriber> GetSubscribersFromDataTable()
    {
        LoadSubscribers();
        return DataTableToList(_subscriberTable);
    }
    
    private List<Subscriber> DataTableToList(DataTable dataTable)
    {
        var subscribers = new List<Subscriber>();
        foreach (DataRow row in dataTable.Rows)
        {
            subscribers.Add(new Subscriber
            {
                Id = Convert.ToInt32(row["Id"]),
                Name = row["Имя"].ToString() ?? "Нет",
                Phone = row["Телефон"].ToString() ?? "Нет",
                IsPremium = Convert.ToBoolean(row["Премиум"]),
                Type = Enum.TryParse(row["Тип"].ToString(), out SubscriberType type) ? type : SubscriberType.Физическое,
                RegistrationDate = DateOnly.FromDateTime(Convert.ToDateTime(row["ДатаРегистрации"]))
            });
        }

        return subscribers;
    }

    public List<Subscriber> GetByName(string name)
    {
        var subscribers = new List<Subscriber>();
        NewConnection(connection =>
        {
            using (var adapter = new SqlDataAdapter(
                       "SELECT * FROM Абонент WHERE Имя like '%' + @Имя + '%'",
                       connection))
            {
                adapter.SelectCommand.Parameters.AddWithValue("@Имя", name);
                var dataTable = new DataTable();
                adapter.Fill(dataTable);
                subscribers = DataTableToList(dataTable);
            }
        });
        return subscribers;
    }

}