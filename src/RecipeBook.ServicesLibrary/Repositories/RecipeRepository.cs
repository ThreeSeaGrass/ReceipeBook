using Dapper;
using RecipeBook.ServicesLibrary.Entities;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace RecipeBook.ServiceLibrary.Repositories
{
    public interface IRecipeRepository
    {
        Task<int> InsertAsync(RecipeEntity entity);
    }
    public class RecipeRepository : IRecipeRepository
    {
        private readonly IIngredientRepository _ingredientRepository;
        private readonly IInstructionRepository _instructionRepository;

        public RecipeRepository(IIngredientRepository ingredientRepository,
            IInstructionRepository instructionRepository)
        {
            this._ingredientRepository = ingredientRepository;
            this._instructionRepository = instructionRepository;
        }
        public async Task<int> InsertAsync(RecipeEntity entity)
        {
            using (var connection = new SqlConnection("Data Source=*****; Initial Catalog=RecipeBook;User Id=sa;Password=*****;MultipleActiveResultSets=true"))
            {
                await connection.OpenAsync();
                using (var transaction = await connection.BeginTransactionAsync())
                {
                    var rowsAffected = await connection.ExecuteAsync(@"
                    INSERT INTO [dbo].[Recipes]
                                ([Id]
                                ,[Title]
                                ,[Description]
                                ,[Logo]
                                ,[CreatedDate])
                            VALUES
                                (@Id
                                ,@Title
                                ,@Description
                                ,@Logo
                                ,@CreateDate)",
                                        new
                                        {
                                            entity.Id,
                                            entity.Title,
                                            entity.Description,
                                            entity.Logo,
                                            entity.CreateDate
                                        }, transaction: transaction);
                    rowsAffected += await _ingredientRepository.InsertAsync(connection, transaction, entity.Ingredients);
                    rowsAffected += await _instructionRepository.InsertAsync(connection, transaction, entity.Instructions);

                    transaction.Commit();
                    return rowsAffected;
                }
            }
        }
    }
}
