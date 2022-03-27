using System;

namespace Olympiad.ControlPanel.Components.UsersTable;

public record UsersPartSelectionModel
{
    private int offset;
    public int Offset
    {
        get => offset;
        set
        {
            offset = Math.Max(0, value);
        }
    }
    private int limit = 10;
    public int Limit
    {
        get => limit;
        set
        {
            if (value < 1)
            {
                value = 10;
            }
            else if (value > 100)
            {
                value = 100;
            }
            limit = value;
        }
    }
}
