using System;
using System.Collections.Generic;

namespace UsersAPI.UtilClasses
{
    public class CustomResponse<T>
    {
        public int errorNumber;
        public List<string> messages;
        public ICollection<T> results;

        public CustomResponse(ICollection<T> results)
        {
            messages = new List<string>();
            this.results = results;
        }

        public CustomResponse(T result)
        {
            messages = new List<string>();
            results = new List<T>();
            results.Add(result);
        }

        public CustomResponse()
        {
            messages = new List<string>();
            this.results = new List<T>();
        }

        public void addMessage(string message)
        {
            messages.Add(message);
        }

        public void addResult(T result)
        {
            results.Add(result);
        }

        public void addResult(List<T> _results)
        {
            results = _results;
        }

        public void incError()
        {
            errorNumber += 1;
        }
    }
}
