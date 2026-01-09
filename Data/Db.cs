using Microsoft.Data.Sqlite;

public sealed class Db
{
    private readonly string _cs;

    public Db(string dbPath = "prevname.sqlite")
    {
        _cs = new SqliteConnectionStringBuilder { DataSource = dbPath }.ToString();
        Init();
    }

    private void Init()
    {
        using var con = new SqliteConnection(_cs);
        con.Open();

        using var cmd = con.CreateCommand();
        cmd.CommandText = @"
CREATE TABLE IF NOT EXISTS name_history (
  id INTEGER PRIMARY KEY AUTOINCREMENT,
  user_id TEXT NOT NULL,
  old_name TEXT NOT NULL,
  new_name TEXT NOT NULL,
  changed_at INTEGER NOT NULL
);

CREATE INDEX IF NOT EXISTS idx_name_history_user_id
ON name_history(user_id);
";
        cmd.ExecuteNonQuery();
    }

    public void AddHistory(ulong userId, string oldName, string newName, long changedAtUnix)
    {
        // On évite les doublons inutiles (ex: events multiples)
        if (string.Equals(oldName, newName, StringComparison.Ordinal))
            return;

        using var con = new SqliteConnection(_cs);
        con.Open();

        using var cmd = con.CreateCommand();
        cmd.CommandText = @"
INSERT INTO name_history (user_id, old_name, new_name, changed_at)
VALUES ($uid, $old, $new, $t);
";
        cmd.Parameters.AddWithValue("$uid", userId.ToString());
        cmd.Parameters.AddWithValue("$old", oldName);
        cmd.Parameters.AddWithValue("$new", newName);
        cmd.Parameters.AddWithValue("$t", changedAtUnix);

        cmd.ExecuteNonQuery();
    }

    public List<(string OldName, string NewName, long ChangedAt)> GetHistory(ulong userId, int limit = 25)
    {
        var list = new List<(string, string, long)>();

        using var con = new SqliteConnection(_cs);
        con.Open();

        using var cmd = con.CreateCommand();
        cmd.CommandText = @"
SELECT old_name, new_name, changed_at
FROM name_history
WHERE user_id = $uid
ORDER BY changed_at DESC
LIMIT $limit;
";
        cmd.Parameters.AddWithValue("$uid", userId.ToString());
        cmd.Parameters.AddWithValue("$limit", limit);

        using var r = cmd.ExecuteReader();
        while (r.Read())
        {
            var oldN = r.GetString(0);
            var newN = r.GetString(1);
            var t = r.GetInt64(2);
            list.Add((oldN, newN, t));
        }

        return list;
    }

    public int ClearHistory(ulong userId)
    {
        using var con = new SqliteConnection(_cs);
        con.Open();

        using var cmd = con.CreateCommand();
        cmd.CommandText = "DELETE FROM name_history WHERE user_id = $uid;";
        cmd.Parameters.AddWithValue("$uid", userId.ToString());
        return cmd.ExecuteNonQuery();
    }
}
