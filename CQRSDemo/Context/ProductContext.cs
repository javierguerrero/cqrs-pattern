using CQRSDemo.Models;
using Microsoft.Data.SqlClient;
using System.Diagnostics;

namespace CQRSDemo.Context
{
    public class ProductContext : IProductContext
    {
        private readonly string ConnectionString;

        public ProductContext(string connectionString) => ConnectionString = connectionString;

        public async Task<int> Add(Product product)
        {
            Object CommandExecutionResult = null;
            var Connection = new SqlConnection(ConnectionString);
            try
            {
                var Command = Connection.CreateCommand();
                Command.CommandType = System.Data.CommandType.Text;
                Command.CommandText = "Insert into Products (" +
                $"{nameof(Product.Name)}, " +
                $"{nameof(Product.QuantityPerUnit)}, " +
                $"{nameof(Product.Description)}, " +
                $"{nameof(Product.UnitPrice)}, " +
                $"{nameof(Product.UnitsInStock)}, " +
                $"{nameof(Product.UnitsOnOrder)}, " +
                $"{nameof(Product.ReorderLevel)}, " +
                $"{nameof(Product.Discontinued)}) " +
                $"Values(" +
                $"@{nameof(Product.Name)}, " +
                $"@{nameof(Product.QuantityPerUnit)}, " +
                $"@{nameof(Product.Description)}, " +
                $"@{nameof(Product.UnitPrice)}, " +
                $"@{nameof(Product.UnitsInStock)}, " +
                $"@{nameof(Product.UnitsOnOrder)}, " +
                $"@{nameof(Product.ReorderLevel)}, " +
                $"@{nameof(Product.Discontinued)});" +
                "select @@Identity;";
                SetSqlParameters(Command.Parameters, product);
                await Connection.OpenAsync();
                CommandExecutionResult = await Command.ExecuteScalarAsync();
                await Command.DisposeAsync();
            }
            catch (Exception ex)
            {
                // Procesar la excepción
                Debug.WriteLine(ex.Message);
            }
            await Connection.DisposeAsync();

            return CommandExecutionResult == null ? -1 : Convert.ToInt32(CommandExecutionResult);
        }

        public async Task<bool> Update(Product product)
        {
            Object CommandExecutionResult = null;
            var Connection = new SqlConnection(ConnectionString);
            try
            {
                var Command = Connection.CreateCommand();
                Command.CommandType = System.Data.CommandType.Text;
                Command.CommandText = "Update Products set " +
                $"{nameof(Product.Name)} = " +
                $"@{nameof(Product.Name)}, " +
                $"{nameof(Product.QuantityPerUnit)} = " +
                $"@{nameof(Product.QuantityPerUnit)}, " +
                $"{nameof(Product.Description)} = " +
                $"@{nameof(Product.Description)}," +
                $"{nameof(Product.UnitPrice)} = " +
                $"@{nameof(Product.UnitPrice)}, " +
                $"{nameof(Product.UnitsInStock)} = " +
                $"@{nameof(Product.UnitsInStock)}, " +
                $"{nameof(Product.UnitsOnOrder)} = " +
                $"@{nameof(Product.UnitsOnOrder)}," +
                $"{nameof(Product.ReorderLevel)} = " +
                $"@{nameof(Product.ReorderLevel)}, " +
                $"{nameof(Product.Discontinued)} = " +
                $"@{nameof(Product.Discontinued)} " +
                $"Where {nameof(Product.Id)} = " +
                $"@{nameof(Product.Id)}";

                SetSqlParameters(Command.Parameters, product);
                Command.Parameters.AddWithValue(
                $"@{nameof(Product.Id)}", product.Id);
                await Connection.OpenAsync();
                CommandExecutionResult = await Command.ExecuteNonQueryAsync();
                await Command.DisposeAsync();
            }
            catch (Exception ex)
            {
                // Procesar la excepción
                Debug.WriteLine(ex.Message);
            }
            await Connection.DisposeAsync();
            return CommandExecutionResult ==
            null ? false : ((int)CommandExecutionResult == 1);
        }

        public async Task<bool> Remove(int id)
        {
            Object CommandExecutionResult = null;
            var Connection = new SqlConnection(ConnectionString);
            try
            {
                var Command = Connection.CreateCommand();
                Command.CommandType = System.Data.CommandType.Text;
                Command.CommandText =
                $"Delete from Products where " +

                $"{nameof(Product.Id)} = @{nameof(Product.Id)};";

                Command.Parameters.AddWithValue(
                $"@{nameof(Product.Id)}", id);
                await Connection.OpenAsync();
                CommandExecutionResult = await Command.ExecuteNonQueryAsync();
                await Command.DisposeAsync();
            }
            catch (Exception ex)
            {
                // Procesar la excepción
                Debug.WriteLine(ex.Message);
            }
            await Connection.DisposeAsync();
            return CommandExecutionResult ==
            null ? false : ((int)CommandExecutionResult == 1);
        }

        public async Task<IEnumerable<Product>> GetAll()
        {
            List<Product> Products = null;
            var Connection = new SqlConnection(ConnectionString);

            try
            {
                var Command = Connection.CreateCommand();
                Command.CommandType = System.Data.CommandType.Text;
                Command.CommandText = "Select * from Products;";
                await Connection.OpenAsync();
                var Reader = await Command.ExecuteReaderAsync();
                if (Reader != null)
                {
                    Products = new List<Product>();
                    while (await Reader.ReadAsync())
                    {
                        Products.Add(GetProduct(Reader));
                    }

                    await Reader.DisposeAsync();
                }
                await Reader.DisposeAsync();
                await Command.DisposeAsync();
            }
            catch (Exception ex)
            {
                // Procesar la excepción
                Debug.WriteLine(ex.Message);
            }
            await Connection.DisposeAsync();
            return Products;
        }

        public async Task<Product> GetById(int id)
        {
            Product Product = null;
            var Connection = new SqlConnection(ConnectionString);
            try
            {
                var Command = Connection.CreateCommand();
                Command.CommandType = System.Data.CommandType.Text;
                Command.CommandText = "Select * from Products where " +
                $"{nameof(Product.Id)} = @{nameof(Product.Id)};";
                Command.Parameters.AddWithValue(
                $"@{nameof(Product.Id)}", id);
                await Connection.OpenAsync();
                var Reader = await Command.ExecuteReaderAsync();
                if (Reader != null)
                {
                    await Reader.ReadAsync();
                    Product = GetProduct(Reader);
                    await Reader.DisposeAsync();
                }
                await Reader.DisposeAsync();
                await Command.DisposeAsync();
            }
            catch (Exception ex)
            {
                // Procesar la excepción
                Debug.WriteLine(ex.Message);
            }
            await Connection.DisposeAsync();
            return Product;
        }

        private void SetSqlParameters(SqlParameterCollection parameters, Product product)
        {
            parameters.AddWithValue($"@{nameof(Product.Name)}", product.Name);
            parameters.AddWithValue($"@{nameof(Product.QuantityPerUnit)}", product.QuantityPerUnit);
            parameters.AddWithValue($"@{nameof(Product.Description)}", product.Description);
            parameters.AddWithValue($"@{nameof(Product.UnitPrice)}", product.UnitPrice);
            parameters.AddWithValue($"@{nameof(Product.UnitsInStock)}", product.UnitsInStock);
            parameters.AddWithValue($"@{nameof(Product.UnitsOnOrder)}", product.UnitsOnOrder);
            parameters.AddWithValue($"@{nameof(Product.ReorderLevel)}", product.ReorderLevel);
            parameters.AddWithValue($"@{nameof(Product.Discontinued)}", product.Discontinued);
        }

        private Product GetProduct(SqlDataReader reader)
        {
            return new Product
            {
                Id = reader.GetInt32(reader.GetOrdinal(nameof(Product.Id))),
                Name = reader.GetString(reader.GetOrdinal(nameof(Product.Name))),
                QuantityPerUnit = reader.GetString(reader.GetOrdinal(nameof(Product.QuantityPerUnit))),
                Description = reader.GetString(reader.GetOrdinal(nameof(Product.Description))),
                UnitPrice = reader.GetDecimal(reader.GetOrdinal(nameof(Product.UnitPrice))),
                UnitsInStock = reader.GetInt32(reader.GetOrdinal(nameof(Product.UnitsInStock))),
                UnitsOnOrder = reader.GetInt32(reader.GetOrdinal(nameof(Product.UnitsOnOrder))),
                ReorderLevel = reader.GetInt32(reader.GetOrdinal(nameof(Product.ReorderLevel))),
                Discontinued = reader.GetBoolean(reader.GetOrdinal(nameof(Product.Discontinued)))
            };
        }
    }
}