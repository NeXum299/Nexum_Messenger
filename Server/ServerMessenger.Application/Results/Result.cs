using System;
using System.Collections.Generic;

namespace ServerMessenger.Application.Results
{
    /// <summary>Класс-обертка для возврата результата операции, содержащий статус выполнения и возможные ошибки.</summary>
    public class Result
    {
        /// <summary>Получает значение, указывающее, успешно ли выполнена операция.</summary>
        public bool Success { get; }

        /// <summary>Получает коллекцию сообщений об ошибках, если операция завершилась неудачно.</summary>
        public IEnumerable<string> Errors { get; }

        /// <summary>Получает значение, указывающее, завершилась ли операция неудачей (обратное свойству Success).</summary>
        public bool Fail => !Success;

        /// <summary>Инициализирует новый экземпляр класса <see cref="Result"/>.</summary>
        /// <param name="success">Статус выполнения операции.</param>
        /// <param name="errors">Коллекция сообщений об ошибках.</param>
        protected Result(bool success, IEnumerable<string> errors)
        {
            Success = success;
            Errors = errors ?? Array.Empty<string>();
        }

        /// <summary>Создает успешный результат без ошибок.</summary>
        /// <returns>Успешный результат.</returns>
        public static Result Ok() => new Result(true, Array.Empty<string>());

        /// <summary>Создает неудачный результат с коллекцией ошибок.</summary>
        /// <param name="errors">Коллекция сообщений об ошибках.</param>
        /// <returns>Неудачный результат с указанными ошибками.</returns>
        public static Result Error(IEnumerable<string> errors) => new Result(false, errors);

        /// <summary>Создает неудачный результат с одной ошибкой.</summary>
        /// <param name="error">Сообщение об ошибке.</param>
        /// <returns>Неудачный результат с указанной ошибкой.</returns>
        public static Result Error(string error) => new Result(false, [error]);

        /// <summary>Создает успешный результат с возвращаемым значением.</summary>
        /// <typeparam name="T">Тип возвращаемого значения.</typeparam>
        /// <param name="value">Возвращаемое значение.</param>
        /// <returns>Успешный результат с указанным значением.</returns>
        public static Result<T> Ok<T>(T value) => new Result<T>(value, true, Array.Empty<string>());

        /// <summary>Создает неудачный результат с возвращаемым значением и коллекцией ошибок.</summary>
        /// <typeparam name="T">Тип возвращаемого значения.</typeparam>
        /// <param name="value">Возвращаемое значение.</param>
        /// <param name="errors">Коллекция сообщений об ошибках.</param>
        /// <returns>Неудачный результат с указанным значением и ошибками.</returns>
        public static Result<T> Error<T>(T value, IEnumerable<string> errors) => new Result<T>(value, false, errors);

        /// <summary>Создает неудачный результат с возвращаемым значением и одной ошибкой.</summary>
        /// <typeparam name="T">Тип возвращаемого значения.</typeparam>
        /// <param name="value">Возвращаемое значение.</param>
        /// <param name="error">Сообщение об ошибке.</param>
        /// <returns>Неудачный результат с указанным значением и ошибкой.</returns>
        public static Result<T> Error<T>(T value, string error) => new Result<T>(value, false, [error]);
    }

    /// <summary>Обобщенная версия класса <see cref="Result"/>, содержащая возвращаемое значение.</summary>
    /// <typeparam name="T">Тип возвращаемого значения.</typeparam>
    public class Result<T> : Result
    {
        /// <summary>Получает возвращаемое значение операции.</summary>
        public T Value { get; }

        /// <summary>Инициализирует новый экземпляр класса <see cref="Result{T}"/>.</summary>
        /// <param name="value">Возвращаемое значение.</param>
        /// <param name="isSuccess">Статус выполнения операции.</param>
        /// <param name="errors">Коллекция сообщений об ошибках.</param>
        protected internal Result(T value, bool isSuccess, IEnumerable<string> errors) : base(isSuccess, errors)
        {
            Value = value;
        }
    }
}
