-- Books
INSERT INTO "Books" ("Id", "Title", "PublishedOn") VALUES('734c6506-a293-4d78-b04d-08dab93deb38'::uuid, 'Book 101', '2021-01-24 00:00:00.000 -0500');
INSERT INTO "Books" ("Id", "Title", "PublishedOn") VALUES('3e8ec853-8ac5-4d28-b04e-08dab93deb38'::uuid, 'Book 102', '2021-04-26 00:00:00.000 -0500');
INSERT INTO "Books" ("Id", "Title", "PublishedOn") VALUES('bc651af7-1de2-4a79-b04f-08dab93deb38'::uuid, 'Book 203', '2022-07-13 00:00:00.000 -0500');
INSERT INTO "Books" ("Id", "Title", "PublishedOn") VALUES('0acf646f-5a7f-4f5c-b050-08dab93deb38'::uuid, 'Book 204', '2022-10-20 00:00:00.000 -0500');

-- Reviews
INSERT INTO "Reviews" ("Id", "Comment", "Rating", "BookId") VALUES('f3f8cb55-d333-4359-1741-08dabb9de4cd'::uuid, 'Comment 01', 1, '3e8ec853-8ac5-4d28-b04e-08dab93deb38'::uuid);
INSERT INTO "Reviews" ("Id", "Comment", "Rating", "BookId") VALUES('92e5fd7a-efee-41f5-1742-08dabb9de4cd'::uuid, 'Comment 02', 2, '3e8ec853-8ac5-4d28-b04e-08dab93deb38'::uuid);
INSERT INTO "Reviews" ("Id", "Comment", "Rating", "BookId") VALUES('e6404220-a6fd-49d7-1743-08dabb9de4cd'::uuid, 'Comment 03', 3, '3e8ec853-8ac5-4d28-b04e-08dab93deb38'::uuid);
INSERT INTO "Reviews" ("Id", "Comment", "Rating", "BookId") VALUES('3b73c3a1-bdee-4af7-1744-08dabb9de4cd'::uuid, 'Comment 04', 4, 'bc651af7-1de2-4a79-b04f-08dab93deb38'::uuid);
INSERT INTO "Reviews" ("Id", "Comment", "Rating", "BookId") VALUES('d59735ec-2221-4613-1745-08dabb9de4cd'::uuid, 'Comment 05', 5, 'bc651af7-1de2-4a79-b04f-08dab93deb38'::uuid);
INSERT INTO "Reviews" ("Id", "Comment", "Rating", "BookId") VALUES('bff16fdc-a7c9-49ad-1746-08dabb9de4cd'::uuid, 'Comment 06', 5, '0acf646f-5a7f-4f5c-b050-08dab93deb38'::uuid);
INSERT INTO "Reviews" ("Id", "Comment", "Rating", "BookId") VALUES('92e0f255-ef02-42a5-1747-08dabb9de4cd'::uuid, 'Comment 07', 4, '0acf646f-5a7f-4f5c-b050-08dab93deb38'::uuid);
INSERT INTO "Reviews" ("Id", "Comment", "Rating", "BookId") VALUES('9dd18902-3398-47ef-1748-08dabb9de4cd'::uuid, 'Comment 08', 3, '0acf646f-5a7f-4f5c-b050-08dab93deb38'::uuid);
