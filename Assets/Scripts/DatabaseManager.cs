using System.Collections.Generic;
using System.IO;
using SQLite;
using UnityEngine;

public class HighScore
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    
    public string PlayerName { get; set; }
    public int Score { get; set; }
    public float CompletionTime { get; set; }
}

public class DatabaseManager : MonoBehaviour
{
    public static DatabaseManager Instance { get; private set; }
    
    private string dbPath;
    private SQLiteConnection dbConnection;
    
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        SetDatabasePath();
        InitializeDatabase();
    }
    
    void SetDatabasePath()
    {
        dbPath = Path.Combine(Application.persistentDataPath, "gamedata.db");
    }
    
    void InitializeDatabase()
    {
        SQLitePCL.Batteries_V2.Init();
        dbConnection = new SQLiteConnection(dbPath);
        CreateHighScoresTable();
    }
    
    void CreateHighScoresTable()
    {
        dbConnection.CreateTable<HighScore>();
    }

    public void SaveScore(string playerName, int score, float completionTime)
    {
        dbConnection.Insert(new HighScore
        {
            PlayerName = playerName,
            Score = score,
            CompletionTime = completionTime
        });
    }

    public List<HighScore> GetTopScores(int limit = 10)
    {
        return dbConnection.Query<HighScore>(
            $"SELECT * FROM HighScore ORDER BY CompletionTime ASC LIMIT {limit}"
        );
    }
}