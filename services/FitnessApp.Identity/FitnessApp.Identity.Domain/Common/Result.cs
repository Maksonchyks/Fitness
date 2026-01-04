using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessApp.Identity.Domain.Common
{
    public class Result
    {
        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;
        public string Error { get; }

        protected Result(bool isSuccess, string error)
        {
            IsSuccess = isSuccess;
            Error = error;
        }

        public static Result Success() => new(true, string.Empty);
        public static Result Failure(string error) => new(false, error);

        // Для зручності: створення generic Result
        public static Result<T> Success<T>(T value) => Result<T>.Success(value);
        public static Result<T> Failure<T>(string error) => Result<T>.Failure(error);
    }

    // Generic Result для повернення значень
    public class Result<T> : Result
    {
        public T Value { get; }

        private Result(bool isSuccess, T value, string error)
            : base(isSuccess, error)
        {
            Value = value;
        }

        public new static Result<T> Success(T value) => new(true, value, string.Empty);
        public new static Result<T> Failure(string error) => new(false, default!, error);

        public static implicit operator Result<T>(T value) => Success(value);
    }
}
