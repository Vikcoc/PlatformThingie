﻿using UsersDbComponent.seeding;

var seeder = new SeedAuthClaimNames();

foreach(var seed in seeder.Select(x => x.Key))
    Console.WriteLine(seed);