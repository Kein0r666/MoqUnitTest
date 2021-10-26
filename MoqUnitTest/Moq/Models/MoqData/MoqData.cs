using MoqUnitTest.Moq.Models.Generator;
using MoqUnitTest.Moq.Models.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using UserServiceTest.MoqDB.MoqModels;

namespace MoqUnitTest.Moq.Models
{
    public class MoqData : IDisposable
    {
        public MoqItems Items { get; set; }

        public MoqData()
        {
            Items = new MoqItems();
        }

        /// <summary>
        /// Генерирует мок объект посредством реализации генератора.
        /// </summary>
        /// <typeparam name="TModel">Модель базы данных, генерируемого мок объекта</typeparam>
        /// <param name="moqGenerator">Реализаци генератора</param>
        /// <returns>Мок модель</returns>
        public IMoqModel<TModel> CreateItem<TModel>(IMoqGenerator<TModel> moqGenerator)
            where TModel : class
        {
            var moqModels = CreateGeneratedMoq(moqGenerator);
            Items.Up(moqGenerator.GetType(), Pack(moqModels));
            return moqModels;
        }
        /// <summary>
        /// Генерирует список одинаковых мок объектов
        /// </summary>
        /// <typeparam name="TModel">Модель базы данных</typeparam>
        /// <param name="moqGenerator">Модель для генерации мок объекта</param>
        /// <param name="count">Количестов экзмеляров</param>
        /// <returns>Список созданных мок объектов</returns>
        public List<IMoqModel<TModel>> CreateListItems<TModel>(IMoqGenerator<TModel> moqGenerator, int count = 1)
            where TModel : class
        {
            var generators = new IMoqGenerator<TModel>[count];

            for (var i = 0; i < count; i++)
            {
                generators[i] = (IMoqGenerator<TModel>)Activator.CreateInstance(moqGenerator.GetType());
            }

            var moqModels = CreateListGeneratedMoq(generators);
            Items.Up(moqGenerator.GetType(), Pack(moqModels).ToList());
            return moqModels;
        }
        /// <summary>
        /// Генерирует список мок объектов
        /// </summary>
        /// <typeparam name="TModel">Модель базы данных</typeparam>
        /// <param name="items">Список моделей генераторов мок объектов</param>
        /// <returns>Список сгенерированных мок объектов</returns>
        public List<IMoqModel<TModel>> CreateListItems<TModel>(IMoqGenerator<TModel>[] items)
            where TModel : class
        {
            var moqModels = CreateListGeneratedMoq(items);
            Items.Up(items[0].GetType(), Pack(moqModels).ToList());
            return moqModels;
        }
        /// <summary>
        /// Получение списка мок моделей по их типу.
        /// </summary>
        /// <typeparam name="T">Тип мок модели</typeparam>
        /// <returns>Списка мок моделей</returns>
        public IEnumerable<T> GetElements<T>()
        {
            foreach (var item in Items.Get(typeof(T)))
                yield return (T)item;
        }
        /// <summary>
        /// Создание списка мок объектов
        /// </summary>
        /// <typeparam name="TModel">Модель базы данных для мок модели</typeparam>
        /// <param name="moqGenerator">Реализация генератора</param>
        /// <returns>Список сгенерированных мок моделей</returns>
        public List<IMoqModel<TModel>> CreateListGeneratedMoq<TModel>(IMoqGenerator<TModel>[] moqGenerator)
            where TModel : class
        {
            var response = new List<IMoqModel<TModel>>();

            foreach(var item in moqGenerator)
                response.Add(item.Generate());

            return response;
        }
        /// <summary>
        /// Создание мок объекта
        /// </summary>
        /// <typeparam name="TModel">Модель базы данных для мок модели</typeparam>
        /// <param name="moqGenerator">Реализация генератора</param>
        /// <returns>Сгенерированную мок модель</returns>
        public IMoqModel<TModel> CreateGeneratedMoq<TModel>(IMoqGenerator<TModel> moqGenerator)
            where TModel : class
        {
            return moqGenerator.Generate();
        }
        /// <summary>
        /// Пакует список объектов
        /// </summary>
        /// <typeparam name="TModel">Модель базы данных</typeparam>
        /// <param name="moqModels">Список мок моеделей</param>
        /// <returns>Список запакованных элементов</returns>
        public IEnumerable<object> Pack<TModel>(IEnumerable<IMoqModel<TModel>> moqModels)
            where TModel : class
        {
            foreach (var item in moqModels)
                yield return (object)item;
        }
        /// <summary>
        /// Пакует объект
        /// </summary>
        /// <typeparam name="TModel">Модель базы данных</typeparam>
        /// <param name="moqModels">Список мок моеделей</param>
        /// <returns>Список запакованных элементов</returns>
        public List<object> Pack<TModel>(IMoqModel<TModel> moqModel)
            where TModel : class
        {
            return new List<object>
            {
                moqModel
            };
        }

        public void Dispose()
        {
            Items.Dispose();
        }
    }
}
