using System;

namespace FitnessApp.Communication.Domain.Common
{
    public static class Guard
    {
        public static void AgainstNullOrEmpty(string value, string name)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException($"{name} cannot be null or empty.", name);
        }

        public static void AgainstDefault(Guid value, string name)
        {
            if (value == Guid.Empty)
                throw new ArgumentException($"{name} cannot be a default GUID.", name);
        }
    }

    public class Result
    {
        public bool IsSuccess { get; }
        public string Error { get; }
        public bool IsFailure => !IsSuccess;

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
        public T Value { get; }

        protected Result(T value, bool isSuccess, string error) : base(isSuccess, error)
        {
            Value = value;
        }

        public static Result<T> Success(T value) => new(value, true, string.Empty);
        public static new Result<T> Failure(string error) => new(default!, false, error);
    }
}
