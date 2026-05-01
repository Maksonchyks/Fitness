BEGIN;
DELETE FROM "UserRoles" WHERE "UserId" = (SELECT "Id" FROM "Users" WHERE "Email" = 'musinmaksim28@gmail.com');
INSERT INTO "UserRoles" ("Id", "UserId", "RoleId", "AssignedAt") 
VALUES (
    gen_random_uuid(), 
    (SELECT "Id" FROM "Users" WHERE "Email" = 'musinmaksim28@gmail.com'), 
    '4d4dd954-e07a-4b2a-bf4f-400309d4328a', 
    NOW()
);
COMMIT;
