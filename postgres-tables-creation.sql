-- Books
CREATE TABLE public."Books" (
  "Id" uuid NOT NULL,
  "Title" text NOT NULL,
  "PublishedOn" timestamptz NOT NULL,
  CONSTRAINT "PK_Books" PRIMARY KEY ("Id")
);

-- Reviews
CREATE TABLE public."Reviews" (
  "Id" uuid NOT NULL,
  "Comment" text NOT NULL,
  "Rating" int4 NOT NULL,
  "BookId" uuid NOT NULL,
  CONSTRAINT "PK_Reviews" PRIMARY KEY ("Id"),
  CONSTRAINT "FK_Reviews_Books_BookId" FOREIGN KEY ("BookId") REFERENCES public."Books"("Id") ON DELETE CASCADE
);
CREATE INDEX "IX_Reviews_BookId" ON public."Reviews" USING btree ("BookId");