using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyHttpServer.Template;

public class TemplateEngine
{

    private Dictionary<string, object> InsertDict;
    private string Template;

    public string GetResult()
    {
        return Template;
    }

    public void SetVariables(string template, Dictionary<string, object> insert = null)
    {
        Template = template;
        InsertDict = insert;
    }


    public void Render()
    {
        if (InsertDict == null) return;

        Stack<string> staples = new Stack<string>();
        Stack<Tuple<Action, int, string[]>> actions = new Stack<Tuple<Action, int, string[]>>();

        int start = 0;
        int end = 0;

        int i = 0;
        while (true)
        {
            if (i >= Template.Length) break;

            if (Template[i] == '{')
            {
                start = end = i++;

                if (Template[i] == '{')
                {
                    staples.Push("{{");
                }
                else if (Template[i] == '%')
                {
                    staples.Push("{%");
                }
            }
            else if (Template[i] == '}' && staples.Count > 0)
            {
                end = ++i;
                if (Template[i] == '}')
                {
                    if (staples.Pop() == "{{")
                    {
                        Insert(start, ref end, Action.Word, null);
                        i = end;
                    }
                }
            }
            else if (Template[i] == '%' && staples.Count > 0)
            {
                end = ++i;
                if (Template[i] == '}' && staples.Count > 0)
                {
                    var staple = staples.Pop();
                    if (staple == "{%")
                    {
                        var action = UnderstandAction(staple, start, end);
                        if (action.Item1 != Action.End && action.Item1 != Action.NotExist)
                        {
                            actions.Push(new Tuple<Action, int, string[]>(action.Item1, start, action.Item2));
                        }
                        else if (action.Item1 != Action.NotExist)
                        {
                            var actionNew = actions.Pop();
                            Insert(actionNew.Item2, ref end, actionNew.Item1, actionNew.Item3);
                            i = end;
                        }
                    }
                }
            }

            if (i >= Template.Length) break;
            i++;
        }
    }

    private (Action, string[]) UnderstandAction(string staple, int start, int end)
    {
        var actionStrings = Template.Substring(start, end - start + 1).Split();

        if (actionStrings[1] == "for")
        {
            return (Action.Cycle, actionStrings);
        }
        else if (actionStrings[1] == "if")
        {
            return (Action.Condition, actionStrings);
        }
        else if (actionStrings[1] == "end")
        {
            return (Action.End, null);
        }
        return (Action.NotExist, null);
    }

    private (string, string) GetParameters(string nameInTemplate)
    {
        string nameClass;
        string namePropertyOrField;
        if (nameInTemplate.Contains('.'))
        {
            var values = nameInTemplate.Split('.');
            nameClass = values[0];
            namePropertyOrField = values[1];
        }
        else
        {
            nameClass = nameInTemplate;
            namePropertyOrField = null;
        }
        return (nameClass, namePropertyOrField);
    }

    enum Action
    {
        Word,
        Cycle,
        Condition,
        End,
        NotExist
    }


    private void Insert(int start, ref int end, Action action, string[] parameters)
    {
        string substring;
        string nameInTemplate;
        string nameClass = null;
        string namePropertyOrField = null;
        object insertInTemplate;
        string substringTemplateWithoutActions = "";

        if (action == Action.Word)
        {
            substring = Template.Substring(start, end - start + 1);
            nameInTemplate = substring.Split()[1];
            (nameClass, namePropertyOrField) = GetParameters(nameInTemplate);
        }
        else if (action == Action.Cycle)
        {
            substring = Template.Substring(start, end - start + 1);
            string forAction = String.Join(' ', parameters);
            substringTemplateWithoutActions = substring.Replace(forAction, null).Replace("{% end %}", null);
            nameInTemplate = parameters[4];
            (nameClass, namePropertyOrField) = GetParameters(nameInTemplate);
        }
        else if (action == Action.Condition)
        {
            substring = Template.Substring(start, end - start + 1);
            string forAction = String.Join(' ', parameters);
            substring = substring.Replace(forAction, null).Replace("{% end %}", null);
            string[] selects = substring.Split("{% else %}", System.StringSplitOptions.RemoveEmptyEntries);
            object select;
            InsertDict.TryGetValue(parameters[2], out select);


            string selectingString = null;
            if (select is Boolean)
            {
                selectingString = (Boolean)select || selects.Length == 1 ? selects[0] : selects[1];
                Template = Template.Remove(start, end - start + 1).Insert(start, selectingString);
            }
            end = selectingString == null ? start : start + selectingString.Length;
            return;
        }

        if (InsertDict.TryGetValue(nameClass, out insertInTemplate))
        {
            Type type = insertInTemplate.GetType();

            if (type == typeof(int) || type == typeof(string))
            {
                Template = Template.Remove(start, end - start + 1).Insert(start, insertInTemplate.ToString());
            }
            else if (type.IsClass && insertInTemplate is not IEnumerable)
            {
                if (namePropertyOrField != null && namePropertyOrField != "")
                {
                    var value = type?.GetProperty(namePropertyOrField)?.GetValue(insertInTemplate);
                    if (value == null)
                        value = type?.GetField(namePropertyOrField)?.GetValue(insertInTemplate);

                    if (value != null)
                    {
                        int length = value.ToString().Length;
                        Template = Template.Remove(start, end - start + 1).Insert(start, value.ToString());
                        end = start + length;
                    }
                }
            }
            else if (insertInTemplate is IEnumerable)
            {
                string nameElement = parameters[2];

                StringBuilder builder = new StringBuilder();
                foreach (var item in (IEnumerable)insertInTemplate)
                {
                    TemplateEngine engine = new TemplateEngine();
                    engine.SetVariables(substringTemplateWithoutActions,
                        new Dictionary<string, object>()
                        { { nameElement, item } });
                    engine.Render();
                    builder.Append(engine.GetResult());
                }
                Template = Template.Remove(start, end - start + 1).Insert(start, builder.ToString());
            }
        }
    }
}