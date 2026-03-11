namespace Demo.Application.Infrastructure.Data;

public class Repository<T> : IRepository<T> where T : BaseEntity
{
    protected readonly DemoDbContext context;
    protected readonly DbSet<T> data;
    protected readonly ILogger<Repository<T>> logger;

    public Repository(DemoDbContext context, ILogger<Repository<T>> logger)
    {
        this.context = context;
        data = this.context.Set<T>();
        this.logger = logger;
    }

    /// <summary>
    /// Adds a new entity to the repository. The entity will not be saved until CommitAsync is called.
    /// </summary>
    /// <param name="entity">Entity to add</param>
    /// <returns>New entity</returns>
    public T Add(T entity) => data.Add(entity).Entity;

    /// <summary>
    /// Adds new entities to the repository. The entities will not be saved until CommitAsync is called.
    /// </summary>
    /// <param name="entities">Parameters of 1 to N entities to add</param>
    public void Add(params T[] entities) => data.AddRange(entities);

    /// <summary>
    /// Adds new entities to the repository. The entities will not be saved until CommitAsync is called.
    /// </summary>
    /// <param name="entities">List entities to add</param>
    public void Add(IEnumerable<T> entities) => data.AddRange(entities);

    /// <summary>
    /// Updates an entity in the repository. The entity will not be saved until CommitAsync is called.
    /// </summary>
    /// <param name="entity">Entity to update</param>
    public void Update(T entity) => data.Update(entity);

    /// <summary>
    /// Updates entities in the repository. The entities will not be saved until CommitAsync is called.
    /// </summary>
    /// <param name="entities">Parameters of 1 to N entities to update</param>
    public void Update(params T[] entities) => data.UpdateRange(entities);

    /// <summary>
    /// Updates entities in the repository. The entities will not be saved until CommitAsync is called.
    /// </summary>
    /// <param name="entities">List of entities to update</param>
    public void Update(IEnumerable<T> entities) => data.UpdateRange(entities);

    /// <summary>
    /// Deletes an entity from the repository. The entity will not be deleted until CommitAsync is called.
    /// </summary>
    /// <param name="entity">Entity to delete</param>
    public void Delete(T entity) => data.Remove(entity);

    /// <summary>
    /// Deletes entities from the repository. The entities will not be deleted until CommitAsync is called.
    /// </summary>
    /// <param name="entities">Parameters of 1 to N entities to delete</param>
    public void Delete(params T[] entities) => data.RemoveRange(entities);

    /// <summary>
    /// Deletes entities from the repository. The entities will not be deleted until CommitAsync is called.
    /// </summary>
    /// <param name="entities">List of entities to delete</param>
    public void Delete(IEnumerable<T> entities) => data.RemoveRange(entities);

    /// <summary>
    /// Commits/Saves all of the pending changes (adds, updates, deletes) in the context.
    /// </summary>
    /// <returns>Number of records affected.</returns>
    public async Task<int> CommitAsync() => await context.SaveChangesAsync();

    /// <summary>
    /// Clears the state all of the currently tracked entities
    /// </summary>
    public void ClearChangeTracker() => context.ChangeTracker.Clear();

    /// <summary>
    /// Saves the entity by adding if new or updating if existing and commits
    /// </summary>
    /// <param name="entity">Entity to save</param>
    /// <returns>Saved entity</returns>
    public async Task<T> SaveEntityAsync(T entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        ClearChangeTracker();
        _ = entity.Id <= 0 ? data.Add(entity) : data.Update(entity);
        await CommitAsync();

        return entity;
    }

    /// <summary>
    /// Saves the entity by adding if new or updating if existing and commits and sets the user as the creator/modifier.
    /// </summary>
    /// <param name="entity">Entity to save</param>
    /// <param name="userId">Id of the user making the request</param>
    /// <returns>Saved entity</returns>
    public async Task<T> SaveEntityAsync(T entity, string userId)
    {
        ArgumentNullException.ThrowIfNull(entity);

        if (entity is BaseAuditEntity audit)
        {
            if (entity.Id <= 0)
            {
                audit.CreatedByUserId = userId;
                audit.CreatedOn = DateTime.UtcNow;
            }
            else
            {
                audit.ModifiedByUserId = userId;
                audit.ModifiedOn = DateTime.UtcNow;
            }
        }

        ClearChangeTracker();
        _ = entity.Id <= 0 ? data.Add(entity) : data.Update(entity);
        await CommitAsync();

        return entity;
    }

    /// <summary>
    /// Saves the entities and commits
    /// </summary>
    /// <param name="entities">List of entities to save</param>
    public async Task SaveEntitiesAsync(IEnumerable<T> entities)
    {
        ArgumentNullException.ThrowIfNull(entities);

        if (entities.Any())
        {
            ClearChangeTracker();
            Update(entities);
            await CommitAsync();
        }
    }

    /// <summary>
    /// Deletes the entity and commits
    /// </summary>
    /// <param name="entity">Entity to delete</param>
    public async Task DeleteEntityAsync(T entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        ClearChangeTracker();
        Delete(entity);
        await CommitAsync();
    }

    /// <summary>
    /// Deletes the entities and commits
    /// </summary>
    /// <param name="entities">List of entities to delete</param>
    public async Task DeleteEntitiesAsync(IEnumerable<T> entities)
    {
        ArgumentNullException.ThrowIfNull(entities);

        if (entities.Any())
        {
            ClearChangeTracker();
            Delete(entities);
            await CommitAsync();
        }
    }

    public async Task ExecuteSqlAsync(string sql)
    {
        if (string.IsNullOrWhiteSpace(sql)) return;
        await context.Database.ExecuteSqlRawAsync(sql);
    }
}
