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
    }

    public class Result<T> : Result
    {
        private readonly T _value;

        public T Value => IsSuccess
            ? _value
            : throw new InvalidOperationException("Cannot access Value on a failed result.");

        private Result(bool isSuccess, T value, string error)
            : base(isSuccess, error)
        {
            _value = value;
        }

        public static Result<T> Success(T value) => new(true, value, string.Empty);

        public new static Result<T> Failure(string error) => new(false, default!, error);

        public static implicit operator Result<T>(T value) => Success(value);
    }
}
