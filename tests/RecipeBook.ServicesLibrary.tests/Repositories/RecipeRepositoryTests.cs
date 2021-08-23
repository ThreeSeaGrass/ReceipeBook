using RecipeBook.ServiceLibrary.Entities;
using RecipeBook.ServiceLibrary.Repositories;
using RecipeBook.ServicesLibrary.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Xunit;

namespace RecipeBook.ServicesLibrary.tests.Repositories
{
    public class RecipeRepositoryTests
    {
        private bool _commitToDatabase = false;
        [Fact]
        public async Task InsertAsync_Success()
        {
            var recipeRepository = new RecipeRepository(new IngredientRepository(), new InstructionRepository());

            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var recipeId = Guid.NewGuid();
                var rowsAffected = await recipeRepository.InsertAsync(new RecipeEntity()
                {
                    Id = recipeId,
                    Title = "Fried Chicken Test",
                    Description = "Fried Chickedn Description Test",
                    Logo = null,
                    CreateDate = DateTimeOffset.UtcNow,
                    Ingredients = new List<IngredientsEntity>()
                    {
                        new IngredientsEntity()
                        {
                           RecipeId = recipeId,
                           Ingredient = "chicken",
                           Unit = "lbs",
                           Quantity = 1,
                           OrdinalPosition = 0
                        }
                    },
                    Instructions = new List<InstructionsEntity>()
                    {
                        new InstructionsEntity()
                        {
                            RecipeId = recipeId,
                            Instruction = "test instructions",
                            OrdinalPosition = 0,
                        }
                    }

                });

                if (_commitToDatabase)
                {
                    scope.Complete();
                }
                Assert.Equal(3, rowsAffected);
            }
        }
    }
}
