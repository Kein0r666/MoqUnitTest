using MoqUnitTest.Moq.Attributes;
using MoqUnitTest.Moq.Models.Extension;
using MoqUnitTest.Moq.Models.Interface;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using UserServiceTest.MoqDB.MoqModels;

namespace MoqUnitTest.Moq.Models.Generator
{
    public abstract class MoqGenerator<T> : IMoqGenerator<T>
        where T : class
    {
        /// <summary>
        /// Метод служит для автоматической генерации случайного объекта, генерирует только простые поля(string, int и т.д.).
        /// Метод желательно всегда переопределять, по причине не полноты данных.
        /// </summary>
        /// <returns>Сгенерированую мок модель</returns>
        public virtual IMoqModel<T> Generate()
        {
            var props = GetType().GetProperties();

            foreach (var property in props)
                if (!Attribute.IsDefined(property, typeof(NonGenerable)))
                    property.SetValue(this, property.Generate(this));

            return (IMoqModel<T>)this;
        }

        public T Generate(T model)
        {
            var props = model.GetType().GetProperties();

            foreach (var property in props)
                if (!Attribute.IsDefined(property, typeof(NonGenerable)))
                    if (!Attribute.IsDefined(property, typeof(NotMappedAttribute)))
                        property.SetValue(model, property.Generate(model));

            return model;
        }
    }

}
