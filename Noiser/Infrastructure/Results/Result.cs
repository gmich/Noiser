﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Noiser
{

    public class Result
    {
        #region Public Properties

        public string ErrorMessage { get; }
        public State State { get; private set; }
        public bool Failure => State != State.Ok;
        public bool Success => State == State.Ok;

        #endregion

        #region Ctor

        protected internal Result(State state, string error)
        {
            State = state;
            DebugArgument.Require.That(() => Success || !string.IsNullOrEmpty(error));
            DebugArgument.Require.That(() => !Success || string.IsNullOrEmpty(error));
            ErrorMessage = error;
        }

        #endregion

        #region Failure

        public static Result<TValue> FailWith<TValue>(State state, string message)
        {
            return new Result<TValue>(default(TValue), state, message);
        }

        public static Result FailWith(State state, string message)
        {
            return new Result(state, message);
        }

        public static Result<TValue> FailWith<TValue>(TValue fallbackValue, State status, string message)
        {
            return new Result<TValue>(fallbackValue, status, message);
        }

        #endregion

        #region Equality and Operators

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            var otherRes = obj as Result;
            if (otherRes == null)
            {
                try
                {
                    return Equals((bool)obj);
                }
                catch { }
                return false;
            }
            return (otherRes.State == this.State);
        }

        public bool Equals(bool success)
        {
            return ((Success && success) || (Failure && !success));
        }

        public static implicit operator bool(Result res)
        {
            return !object.ReferenceEquals(res, null) && res.Success;
        }


        public static bool operator ^(Result result, Result otherRes)
        {
            return result.Equals(otherRes);
        }

        public static bool operator ==(Result result, Result otherRes)
        {
            if (Object.ReferenceEquals(result, otherRes))
            {
                return true;
            }
            if (result == null || otherRes == null)
            {
                return false;
            }
            return result.Equals(otherRes);
        }

        public static bool operator !=(Result result, Result otherRes)
        {
            if (Object.ReferenceEquals(result, otherRes))
            {
                return false;
            }
            if (result == null && otherRes != null)
            {
                return true;
            }
            if (result != null && otherRes == null)
            {
                return true;
            }
            return !result.Equals(otherRes);
        }

        public static Result operator &(Result result, Result otherRes)
        {
            if (Object.ReferenceEquals(result, otherRes))
            {
                return Result.Ok();
            }
            if (Object.ReferenceEquals(result, null) || Object.ReferenceEquals(otherRes, null))
            {
                return FailWith(State.Error, "Result in bitwise & was null");
            }

            if (result.Failure)
            {
                return result;
            }
            else
            {
                return otherRes;
            }
        }

        public static Result operator |(Result result, Result otherRes)
        {
            if (Object.ReferenceEquals(result, null) || Object.ReferenceEquals(otherRes, null))
            {
                return FailWith(State.Error, "Result in bitwise | was null");
            }

            if (result.Failure)
            {
                return otherRes;
            }
            return result;
        }

        public static Result operator !(Result result)
        {
            if (Object.ReferenceEquals(result, null))
            {
                return FailWith(State.Error, "Result in bitwise | was null");
            }

            if (result.State == State.Ok)
            {
                result.State = State.Error;
            }
            else
            {
                result.State = State.Ok;
            }
            return result;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator true(Result res)
        {
            return res.Success;
        }

        public static bool operator false(Result res)
        {
            return res.Failure;
        }

        #endregion

        #region Success

        public static Result Ok()
        {
            return new Result(State.Ok, String.Empty);
        }

        public static Result Ok(Action action)
        {
            action();
            return new Result(State.Ok, String.Empty);
        }

        public static Result<TValue> Ok<TValue>(TValue value)
        {
            return new Result<TValue>(value, State.Ok, String.Empty);
        }
        public static Result<TValue> Ok<TValue>(Func<TValue> valueGetter)
        {
            return new Result<TValue>(valueGetter(), State.Ok, String.Empty);
        }


        #endregion

        #region Assert

        public static Result<TValue> RequireNotNull<TValue>(Func<TValue> valueGetter)
        {
            var value = valueGetter();
            Argument.Require.NotNull(() => value);
            return RequireNotNull(value);
        }

        public static Result<TValue> RequireNotNull<TValue>(TValue value)
        {
            Argument.Require.NotNull(() => value);
            return Ok(value);
        }

        public static Result Test(bool value)
        {
            Argument.Require.That(() => value == true);
            return Result.Ok();
        }

        public static Result<TValue> FailIfNull<TValue>(TValue value, string msg = "value was null")
        {
            if (value == null)
            {
                return FailWith<TValue>(State.NotFound, msg);
            }
            return Ok(value);
        }

        #endregion

        #region Gates

        public static Result OnFirstSuccess(params Func<Result>[] results)
        {
            foreach (var result in results)
            {
                var res = result();
                if (res.Success)
                {
                    return res;
                }
            }
            return Result.FailWith(State.Error, "All results failed");
        }

        public static Result<TValue> OnFirstSuccess<TValue>(params Func<Result<TValue>>[] results)
        {
            foreach (var result in results)
            {
                var res = result();
                if (res.Success)
                {
                    return res;
                }
            }
            return Result.FailWith<TValue>(State.Error, "All results failed");
        }

        public static Result OnAll(params Func<Result>[] results)
        {
            foreach (var result in results)
            {
                var res = result();
                if (res.Failure)
                {
                    return res;
                }
            }
            return Result.Ok();
        }

        #endregion

        #region Misc

        public static Task<Result<TValue>> AsTask<TValue>(Func<Result<TValue>> getAsync)
        {
            return Task.Run(getAsync);
        }

        public static Result Combine(params Result[] results)
        {
            foreach (Result result in results)
            {
                if (result.Failure)
                {
                    return result;
                }
            }
            return Ok();
        }

        public static Result Combine(IEnumerable<Result> results)
        {
            foreach (Result result in results)
            {
                if (result.Failure)
                {
                    return result;
                }
            }
            return Ok();
        }

        public override string ToString()
        {
            return $"Success: { Success } , State: {State}";
        }

        #endregion
    }
}