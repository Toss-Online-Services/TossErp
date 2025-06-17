using POS.Domain.Common;
using POS.Domain.Exceptions;

namespace POS.Domain.AggregatesModel.CustomerAggregate
{
    public class CustomerNote : Entity
    {
        public string Content { get; private set; }
        public string CreatedBy { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? LastModifiedAt { get; private set; }
        public string? LastModifiedBy { get; private set; }
        public bool IsPrivate { get; private set; }
        public string? Category { get; private set; }
        public int Priority { get; private set; }
        public bool IsResolved { get; private set; }
        public DateTime? ResolvedAt { get; private set; }
        public string? ResolvedBy { get; private set; }

        private CustomerNote()
        {
            Content = string.Empty;
            CreatedBy = string.Empty;
            CreatedAt = DateTime.UtcNow;
            IsPrivate = false;
            Priority = 0;
            IsResolved = false;
        }

        public CustomerNote(string content, string createdBy, bool isPrivate = false, 
            string? category = null, int priority = 0)
        {
            if (string.IsNullOrWhiteSpace(content))
                throw new DomainException("Note content cannot be empty");
            if (string.IsNullOrWhiteSpace(createdBy))
                throw new DomainException("Note creator cannot be empty");
            if (priority < 0 || priority > 5)
                throw new DomainException("Priority must be between 0 and 5");

            Id = Guid.NewGuid();
            Content = content;
            CreatedBy = createdBy;
            CreatedAt = DateTime.UtcNow;
            IsPrivate = isPrivate;
            Category = category;
            Priority = priority;
            IsResolved = false;
        }

        public void UpdateContent(string content, string modifiedBy)
        {
            if (string.IsNullOrWhiteSpace(content))
                throw new DomainException("Note content cannot be empty");
            if (string.IsNullOrWhiteSpace(modifiedBy))
                throw new DomainException("Modifier cannot be empty");

            Content = content;
            LastModifiedAt = DateTime.UtcNow;
            LastModifiedBy = modifiedBy;
        }

        public void SetPrivate(bool isPrivate)
        {
            IsPrivate = isPrivate;
            LastModifiedAt = DateTime.UtcNow;
        }

        public void SetCategory(string? category)
        {
            Category = category;
            LastModifiedAt = DateTime.UtcNow;
        }

        public void SetPriority(int priority)
        {
            if (priority < 0 || priority > 5)
                throw new DomainException("Priority must be between 0 and 5");

            Priority = priority;
            LastModifiedAt = DateTime.UtcNow;
        }

        public void Resolve(string resolvedBy)
        {
            if (string.IsNullOrWhiteSpace(resolvedBy))
                throw new DomainException("Resolver cannot be empty");

            IsResolved = true;
            ResolvedAt = DateTime.UtcNow;
            ResolvedBy = resolvedBy;
        }

        public void Unresolve()
        {
            IsResolved = false;
            ResolvedAt = null;
            ResolvedBy = null;
        }
    }
} 
