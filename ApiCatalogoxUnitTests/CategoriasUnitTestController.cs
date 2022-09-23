using ApiCatalogo.Controllers;
using ApiCatalogo.DTOs;
using ApiCatalogo.DTOs.Mappings;
using ApiCatalogo.Repository;
using APICatalogo.Context;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace ApiCatalogoxUnitTests
{
    public class CategoriasUnitTestController
    {
        private IMapper mapper;
        private IUnitOfWork repository;

        public static DbContextOptions<AppDbContext> dbContextOptions { get; }

        public static string connectionString =
           "Server=localhost;DataBase=apicatalogodb;Uid=root;Pwd=semsenha1";

        static CategoriasUnitTestController()
        {
            dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
                .UseMySql(connectionString,
                 ServerVersion.AutoDetect(connectionString))
                .Options;
        }

        public CategoriasUnitTestController()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MappingProfile());
            });
            mapper = config.CreateMapper();

            var context = new AppDbContext(dbContextOptions);

            //DBUnitTestsMockInitializer db = new DBUnitTestsMockInitializer();
            //db.Seed(context);

            repository = new UnitOfWork(context);
        }

        //=======================================================================
        // testes unitários
        // Inicio dos testes : método GET

        [Fact]
        public void GetCategorias_Return_OkResult()
        {
            //Arrange  
            var controller = new CategoriasController(repository, mapper);

            //Act  
            var data = controller.Get();

            //Assert  
            Assert.IsType<List<CategoriaDTO>>(data.Value);
        }

        // GET BadRequest
        [Fact]
        public void GetCategorias_Return_BadRequest()
        {
            //Arrange  
            var controller = new CategoriasController(repository, mapper);

            //Act  
            var data = controller.Get();

            //Assert  
            Assert.IsType<BadRequestResult>(data.Result);
        }


        // GET retornar uma lista de objetos categoria
        [Fact]
        public void GetCategorias_MatchResult()
        {
            //Arrange
            var controller = new CategoriasController(repository, mapper);

            //Act
            var data = controller.Get();

            //Assert
            Assert.IsType<List<CategoriaDTO>>(data.Value);
            var cat = data.Value.Should().BeAssignableTo<List<CategoriaDTO>>().Subject;

            Assert.Equal("Bebidas", cat[0].Nome);
            Assert.Equal("http://www.macoratti.net/Imagens/1.jpg", cat[0].ImagemUrl);

            Assert.Equal("Sobremesas", cat[2].Nome);
            Assert.Equal("http://www.macoratti.net/Imagens/3.jpg", cat[2].ImagemUrl);
        }

        // GET por id // retornar um objeto CategoriaDTO
        [Fact]
        public void GetCategoriaById_Return_OkResult()
        {
            //Arrange  
            var controller = new CategoriasController(repository, mapper);
            var catId = 2;

            //Act  
            var data = controller.Get(catId);

            //Assert
            Assert.IsType<CategoriaDTO>(data.Value);
        }

        // GET por id -> NotFound
        [Fact]
        public void GetCategoriaById_Return_NotFound()
        {
            //Arrange  
            var controller = new CategoriasController(repository, mapper);
            var catId = 999;

            //Act  
            var data = controller.Get(catId);

            //Assert
            Assert.IsType<NotFoundResult>(data.Result);
        }

        // POST - CreatedResult
        [Fact]
        public void Post_Categoria_AddValidData_Return_CreatedResult()
        {
            //Arrange  
            var controller = new CategoriasController(repository, mapper);

            var cat = new CategoriaDTO() { Nome = "Teste Unitario 1", ImagemUrl = "testecat.jpg"};

            //Act  
            var data = controller.Post(cat);

            //Assert
            Assert.IsType<CreatedAtRouteResult>(data);
        }

        // PUT -- alterar objeto categoria
        [Fact]
        public void Put_Categoria_Update_ValidData_Return_OkResult()
        {
            //Arrange
            var controller = new CategoriasController(repository, mapper);
            var catId = 3;

            //Act
            var existingPost = controller.Get(catId);
            var result = existingPost.Value.Should().BeAssignableTo<CategoriaDTO>().Subject;

            var catDto = new CategoriaDTO();
            catDto.CategoriaId = catId;
            catDto.Nome = "Categoria atualizada - testes 1";
            catDto.ImagemUrl = result.ImagemUrl;

            var updatedData = controller.Put(catId, catDto);

            //Assert
            Assert.IsType<OkResult>(updatedData);
        }

        // DELETE - id -
        [Fact]
        public void Delete_Categoria_Return_OkResult()
        {
            //Arrange  
            var controller = new CategoriasController(repository, mapper);
            var catId = 5;

            //Act  
            var data = controller.Delete(catId);

            //Assert
            Assert.IsType<CategoriaDTO>(data.Value);
        }
    }
}
