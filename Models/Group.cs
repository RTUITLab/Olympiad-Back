﻿using Models.Exercises;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    public class Group
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public string InviteToken { get; set; }
        public string LessonsTime { get; set; }
        public List<UserToGroup> UserToGroups { get; set; }
        public List<GroupToCourse> GroupToCourses { get; set; }
        public List<Challenge> Challenges { get; set; }
    }
}
