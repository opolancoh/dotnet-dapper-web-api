using DapperExample.Web.Data.Schemas;
using DapperExample.Web.Models;
using FluentMigrator;

namespace DapperExample.Web.Migrations;

[Migration(1)]
public class AddTablesMigration : Migration
{
    public override void Up()
    {
        Create.Table(BookSchema.Table)
            .WithColumn(BookSchema.Columns.Id).AsGuid().NotNullable().PrimaryKey()
            .WithColumn(BookSchema.Columns.Title).AsString().NotNullable()
            .WithColumn(BookSchema.Columns.PublishedOn).AsDateTime().NotNullable();
        
        Create.Table(ReviewSchema.Table)
            .WithColumn(ReviewSchema.Columns.Id).AsGuid().NotNullable().PrimaryKey()
            .WithColumn(ReviewSchema.Columns.Comment).AsString().NotNullable()
            .WithColumn(ReviewSchema.Columns.Rating).AsInt32().NotNullable()
            .WithColumn(ReviewSchema.Columns.BookId).AsGuid().NotNullable().ForeignKey(ReviewSchema.Table, BookSchema.Columns.Id);
    }
    
    public override void Down()
    {
          Delete.Table(BookSchema.Table);
          Delete.Table(ReviewSchema.Table);
    }
}