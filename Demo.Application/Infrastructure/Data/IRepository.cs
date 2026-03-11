namespace Demo.Application.Infrastructure.Data;

public interface IRepository<T> where T : class
{
    /// <summary>
    /// Adds a new entity to the repository. The entity will not be saved until CommitAsync is called.
    /// </summary>
    /// <param name="entity">Entity to add</param>
    /// <returns>New entity</returns>
    T Add(T entity);

    /// <summary>
    /// Adds new entities to the repository. The entities will not be saved until CommitAsync is called.
    /// </summary>
    /// <param name="entities">Parameters of 1 to N entities to add</param>
    void Add(params T[] entities);

    /// <summary>
    /// Adds new entities to the repository. The entities will not be saved until CommitAsync is called.
    /// </summary>
    /// <param name="entities">List entities to add</param>
    void Add(IEnumerable<T> entities);

    /// <summary>
    /// Updates an entity in the repository. The entity will not be saved until CommitAsync is called.
    /// </summary>
    /// <param name="entity">Entity to update</param>
    void Update(T entity);

    /// <summary>
    /// Updates entities in the repository. The entities will not be saved until CommitAsync is called.
    /// </summary>
    /// <param name="entities">Parameters of 1 to N entities to update</param>
    void Update(params T[] entities);

    /// <summary>
    /// Updates entities in the repository. The entities will not be saved until CommitAsync is called.
    /// </summary>
    /// <param name="entities">List of entities to update</param>
    void Update(IEnumerable<T> entities);

    /// <summary>
    /// Deletes an entity from the repository. The entity will not be deleted until CommitAsync is called.
    /// </summary>
    /// <param name="entity">Entity to delete</param>
    void Delete(T entity);

    /// <summary>
    /// Deletes entities from the repository. The entities will not be deleted until CommitAsync is called.
    /// </summary>
    /// <param name="entities">Parameters of 1 to N entities to delete</param>
    void Delete(params T[] entities);

    /// <summary>
    /// Deletes entities from the repository. The entities will not be deleted until CommitAsync is called.
    /// </summary>
    /// <param name="entities">List of entities to delete</param>
    void Delete(IEnumerable<T> entities);

    /// <summary>
    /// Commits/Saves all of the pending changes (adds, updates, deletes) in the context.
    /// </summary>
    /// <returns>Number of records affected.</returns>
    Task<int> CommitAsync();

    /// <summary>
    /// Clears the state all of the currently tracked entities
    /// </summary>
    void ClearChangeTracker();

    /// <summary>
    /// Saves the entity by adding if new or updating if existing and commits
    /// </summary>
    /// <param name="entity">Entity to save</param>
    /// <returns>Saved entity</returns>
    Task<T> SaveEntityAsync(T entity);

    /// <summary>
    /// Saves the entity by adding if new or updating if existing and commits and sets the user as the creator/modifier.
    /// </summary>
    /// <param name="entity">Entity to save</param>
    /// <param name="userId">Id of the user making the request</param>
    /// <returns>Saved entity</returns>
    Task<T> SaveEntityAsync(T entity, string userId);

    /// <summary>
    /// Saves the entities and commits
    /// </summary>
    /// <param name="entities">List of entities to save</param>
    Task SaveEntitiesAsync(IEnumerable<T> entities);

    /// <summary>
    /// Deletes the entity and commits
    /// </summary>
    /// <param name="entity">Entity to delete</param>
    Task DeleteEntityAsync(T entity);

    /// <summary>
    /// Deletes the entities and commits
    /// </summary>
    /// <param name="entities">List of entities to delete</param>
    Task DeleteEntitiesAsync(IEnumerable<T> entities);

    Task ExecuteSqlAsync(string sql);
}
