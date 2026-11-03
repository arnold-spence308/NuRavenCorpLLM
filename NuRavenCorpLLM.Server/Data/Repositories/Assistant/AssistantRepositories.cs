using Microsoft.EntityFrameworkCore;
using NuRavenCorpLLM.Core.Interfaces;
using NuRavenCorpLLM.Core.Interfaces.Repositories.Assistant;
using NuRavenCorpLLM.Entities;
using NuRavenCorpLLM.Infrastructure.Data;

namespace NuRavenCorpLLM.Server.Data.Repositories.Assistant;

public class AssistantConversationRepository : IAssistantConversationRepository
{
    private readonly NuRavenCorpLLM.Infrastructure.Data.ApplicationDbContext _ctx;

    public AssistantConversationRepository(NuRavenCorpLLM.Infrastructure.Data.ApplicationDbContext ctx)
    {
        _ctx = ctx;
    }

    public async Task<AssistantConversation?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _ctx.Set<AssistantConversation>().FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, ct);

    public async Task<AssistantConversation?> GetWithMessagesAsync(Guid conversationId, CancellationToken ct = default)
        => await _ctx.Set<AssistantConversation>()
            .Include(x => x.Messages.Where(m => !m.IsDeleted))
            .FirstOrDefaultAsync(x => x.Id == conversationId && !x.IsDeleted, ct);

    public async Task<List<AssistantConversation>> GetByUserAsync(Guid userId, CancellationToken ct = default)
        => await _ctx.Set<AssistantConversation>()
            .Where(x => x.UserId == userId && !x.IsDeleted)
            .OrderByDescending(x => x.LastMessageAt)
            .ToListAsync(ct);

    public async Task<AssistantConversation> AddAsync(AssistantConversation entity, CancellationToken ct = default)
    {
        await _ctx.Set<AssistantConversation>().AddAsync(entity, ct);
        await _ctx.SaveChangesAsync(ct);
        return entity;
    }

    public async Task<AssistantConversation> UpdateAsync(AssistantConversation entity, CancellationToken ct = default)
    {
        entity.UpdatedAt = DateTime.UtcNow;
        _ctx.Set<AssistantConversation>().Update(entity);
        await _ctx.SaveChangesAsync(ct);
        return entity;
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await GetByIdAsync(id, ct);
        if (entity == null) return;
        entity.IsDeleted = true;
        entity.DeletedAt = DateTime.UtcNow;
        await UpdateAsync(entity, ct);
    }
}

public class AssistantMessageRepository : IAssistantMessageRepository
{
    private readonly NuRavenCorpLLM.Infrastructure.Data.ApplicationDbContext _ctx;

    public AssistantMessageRepository(NuRavenCorpLLM.Infrastructure.Data.ApplicationDbContext ctx)
    {
        _ctx = ctx;
    }

    public async Task<AssistantMessage?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _ctx.Set<AssistantMessage>().FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, ct);

    public async Task<AssistantMessage?> GetLastAssistantMessageAsync(Guid conversationId, CancellationToken ct = default)
        => await _ctx.Set<AssistantMessage>()
            .Where(x => x.ConversationId == conversationId && x.Role == AssistantRole.Assistant && !x.IsDeleted)
            .OrderByDescending(x => x.CreatedAt)
            .FirstOrDefaultAsync(ct);

    public async Task<AssistantMessage> AddAsync(AssistantMessage entity, CancellationToken ct = default)
    {
        await _ctx.Set<AssistantMessage>().AddAsync(entity, ct);
        await _ctx.SaveChangesAsync(ct);
        return entity;
    }

    public async Task<AssistantMessage> UpdateAsync(AssistantMessage entity, CancellationToken ct = default)
    {
        entity.UpdatedAt = DateTime.UtcNow;
        _ctx.Set<AssistantMessage>().Update(entity);
        await _ctx.SaveChangesAsync(ct);
        return entity;
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await GetByIdAsync(id, ct);
        if (entity == null) return;
        entity.IsDeleted = true;
        entity.DeletedAt = DateTime.UtcNow;
        await UpdateAsync(entity, ct);
    }
}

public class AssistantFeedbackRepository : IAssistantFeedbackRepository
{
    private readonly NuRavenCorpLLM.Infrastructure.Data.ApplicationDbContext _ctx;

    public AssistantFeedbackRepository(NuRavenCorpLLM.Infrastructure.Data.ApplicationDbContext ctx)
    {
        _ctx = ctx;
    }

    public async Task<AssistantFeedback?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _ctx.Set<AssistantFeedback>().FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, ct);

    public async Task<List<AssistantFeedback>> GetUnprocessedAsync(CancellationToken ct = default)
        => await _ctx.Set<AssistantFeedback>()
            .Where(x => !x.IsDeleted && !x.IsProcessed)
            .OrderBy(x => x.CreatedAt)
            .ToListAsync(ct);

    public async Task<AssistantFeedback> AddAsync(AssistantFeedback entity, CancellationToken ct = default)
    {
        await _ctx.Set<AssistantFeedback>().AddAsync(entity, ct);
        await _ctx.SaveChangesAsync(ct);
        return entity;
    }

    public async Task<AssistantFeedback> UpdateAsync(AssistantFeedback entity, CancellationToken ct = default)
    {
        entity.UpdatedAt = DateTime.UtcNow;
        _ctx.Set<AssistantFeedback>().Update(entity);
        await _ctx.SaveChangesAsync(ct);
        return entity;
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await GetByIdAsync(id, ct);
        if (entity == null) return;
        entity.IsDeleted = true;
        entity.DeletedAt = DateTime.UtcNow;
        await UpdateAsync(entity, ct);
    }
}

public class AssistantLearningSampleRepository : IAssistantLearningSampleRepository
{
    private readonly NuRavenCorpLLM.Infrastructure.Data.ApplicationDbContext _ctx;

    public AssistantLearningSampleRepository(NuRavenCorpLLM.Infrastructure.Data.ApplicationDbContext ctx)
    {
        _ctx = ctx;
    }

    public async Task<AssistantLearningSample?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _ctx.Set<AssistantLearningSample>().FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, ct);

    public async Task<AssistantLearningSample?> GetByMessageIdAsync(Guid messageId, CancellationToken ct = default)
        => await _ctx.Set<AssistantLearningSample>()
            .FirstOrDefaultAsync(x => x.MessageId == messageId && !x.IsDeleted, ct);

    public async Task<List<AssistantLearningSample>> GetPendingAsync(int batchSize, CancellationToken ct = default)
        => await _ctx.Set<AssistantLearningSample>()
            .Where(x => x.Status == LearningSampleStatus.Pending && !x.IsDeleted)
            .OrderBy(x => x.CreatedAt)
            .Take(batchSize)
            .ToListAsync(ct);

    public async Task<List<AssistantLearningSample>> GetApprovedAsync(DateTime? since = null, CancellationToken ct = default)
    {
        var query = _ctx.Set<AssistantLearningSample>()
            .Where(x => x.Status == LearningSampleStatus.Approved && !x.IsDeleted);

        if (since.HasValue)
            query = query.Where(x => x.UpdatedAt >= since.Value);

        return await query.OrderByDescending(x => x.UpdatedAt).ToListAsync(ct);
    }

    public async Task<AssistantLearningSample> AddAsync(AssistantLearningSample entity, CancellationToken ct = default)
    {
        await _ctx.Set<AssistantLearningSample>().AddAsync(entity, ct);
        await _ctx.SaveChangesAsync(ct);
        return entity;
    }

    public async Task<AssistantLearningSample> UpdateAsync(AssistantLearningSample entity, CancellationToken ct = default)
    {
        entity.UpdatedAt = DateTime.UtcNow;
        _ctx.Set<AssistantLearningSample>().Update(entity);
        await _ctx.SaveChangesAsync(ct);
        return entity;
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await GetByIdAsync(id, ct);
        if (entity == null) return;
        entity.IsDeleted = true;
        entity.DeletedAt = DateTime.UtcNow;
        await UpdateAsync(entity, ct);
    }
}

public class AssistantKnowledgeRepository : IAssistantKnowledgeRepository
{
    private readonly NuRavenCorpLLM.Infrastructure.Data.ApplicationDbContext _ctx;

    public AssistantKnowledgeRepository(NuRavenCorpLLM.Infrastructure.Data.ApplicationDbContext ctx, ILogger<AssistantKnowledgeRepository> logger)
    {
        _ctx = ctx;
    }

    public async Task<AssistantKnowledge?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _ctx.Set<AssistantKnowledge>().FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task<AssistantKnowledge> AddAsync(AssistantKnowledge entity, CancellationToken ct = default)
    {
        await _ctx.Set<AssistantKnowledge>().AddAsync(entity, ct);
        await _ctx.SaveChangesAsync(ct);
        return entity;
    }

    public async Task<AssistantKnowledge> UpdateAsync(AssistantKnowledge entity, CancellationToken ct = default)
    {
        _ctx.Set<AssistantKnowledge>().Update(entity);
        await _ctx.SaveChangesAsync(ct);
        return entity;
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await GetByIdAsync(id, ct);
        if (entity == null)
        {
            return;
        }

        entity.IsDeleted = true;
        entity.DeletedAt = DateTime.UtcNow;
        _ctx.Set<AssistantKnowledge>().Update(entity);
        await _ctx.SaveChangesAsync(ct);
    }
}
