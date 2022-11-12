namespace DapperExample.Web.Services;

/*public class ReviewService : IReviewService
{
    private readonly ApplicationDbContext _context;
    private readonly DbSet<Review> _entity;

    public ReviewService(ApplicationDbContext context)
    {
        _context = context;
        _entity = context.Reviews;
    }

    public async Task<IEnumerable<ReviewDto>> GetAll()
    {
        var query = GetReviewDtoQuery();

        return await query.ToListAsync();
    }

    public async Task<ReviewDto?> GetById(Guid id)
    {
        var query = GetReviewDtoQuery();

        return await query.SingleOrDefaultAsync(x => x.Id == id);
    }

    public async Task<ReviewDto> Create(ReviewForCreatingDto item)
    {
        var newItem = new Review()
        {
            Id = new Guid(),
            Comment = item.Comment,
            Rating = item.Rating!.Value,
            BookId = item.BookId!.Value
        };

        _entity.Add(newItem);
        await _context.SaveChangesAsync();

        var dto = TransformItemToReviewDto(newItem);

        return dto;
    }

    public async Task<ReviewDto> Update(ReviewForUpdatingDto item)
    {
        var currentItem = _entity.SingleOrDefault(x => x.Id == item.Id);
        if (currentItem == null)
            throw new EntityNotFoundException(item.Id!.Value);

        currentItem.Comment = item.Comment;
        currentItem.Rating = item.Rating!.Value;

        _context.Entry(currentItem).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        var dto = TransformItemToReviewDto(currentItem);

        return dto;
    }

    public async Task Remove(Guid id)
    {
        var item = new Review {Id = id};

        _entity.Remove(item);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ItemExists(id))
            {
                throw new EntityNotFoundException(id);
            }
            else
            {
                throw;
            }
        }
    }

    private bool ItemExists(Guid id)
    {
        return _entity.Any(e => e.Id == id);
    }

    private IQueryable<ReviewDto> GetReviewDtoQuery()
    {
        return _entity
            .AsNoTracking()
            .Select(x => new ReviewDto
            {
                Id = x.Id,
                Comment = x.Comment,
                Rating = x.Rating,
                BookId = x.BookId
            });
    }

    private static ReviewDto TransformItemToReviewDto(Review item)
    {
        return new ReviewDto
        {
            Id = item.Id,
            Comment = item.Comment,
            Rating = item.Rating,
            BookId = item.BookId
        };
    }
}*/