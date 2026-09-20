// Infrastructure/Vector/PgVectorStore.cs
using Microsoft.EntityFrameworkCore;
using NuRavenCorpLLM.Application.Services.Assistant;

namespace NuRavenCorpLLM.Infrastructure.Vector;

public interface IPgVectorStore
{
    Task<IEnumerable<RagHit>> SimilaritySearchAsync(float[] query, int topK, double minScore, CancellationToken ct = default);
}

public record RagHit(Guid Id, string Title, string Content, string Source, string? SourceUrl, double Score);

public class PgVectorStore : IPgVectorStore
{
    private readonly AppDbContext _db;

    public PgVectorStore(AppDbContext db) => _db = db;

    public async Task<IEnumerable<RagHit>> SimilaritySearchAsync(
        float[] query, int topK, double minScore, CancellationToken ct = default)
    {
        // Enable the pgvector extension once per DB: CREATE EXTENSION vector;
        // Column added via migration: embedding vector(1536)
        var vectorLiteral = "[" + string.Join(',', query.Select(f => f.ToString(System.Globalization.CultureInfo.InvariantCulture))) + "]";

        var sql = @"
            SELECT id, title, content, source, source_url,
                   1 - (embedding <=> @p0::vector) AS score
            FROM assistant_knowledge
            WHERE is_public = true AND is_approved = true
              AND embedding IS NOT NULL
            ORDER BY embedding <=> @p0::vector
            LIMIT @p1;";

        var results = new List<RagHit>();
        using var cmd = _db.Database.GetDbConnection().CreateCommand();
        cmd.CommandText = sql;

        var p0 = cmd.CreateParameter(); p0.ParameterName = "@p0"; p0.Value = vectorLiteral; cmd.Parameters.Add(p0);
        var p1 = cmd.CreateParameter(); p1.ParameterName = "@p1"; p1.Value = topK; cmd.Parameters.Add(p1);

        await _db.Database.OpenConnectionAsync(ct);
        using var reader = await cmd.ExecuteReaderAsync(ct);
        while (await reader.ReadAsync(ct))
        {
            var score = reader.GetDouble(5);
            if (score < minScore) continue;
            results.Add(new RagHit(
                reader.GetGuid(0),
                reader.GetString(1),
                reader.GetString(2),
                reader.GetString(3),
                reader.IsDBNull(4) ? null : reader.GetString(4),
                score));
        }
        return results;
    }
}