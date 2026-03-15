-- public.book_types definition

-- Drop table

-- DROP TABLE public.book_types;

CREATE TABLE public.book_types ( id int4 GENERATED ALWAYS AS IDENTITY( INCREMENT BY 1 MINVALUE 1 MAXVALUE 2147483647 START 1 CACHE 1 NO CYCLE) NOT NULL, book_type_name varchar(50) NULL, CONSTRAINT pk_book_types PRIMARY KEY (id));

-- Permissions

ALTER TABLE public.book_types OWNER TO postgres;
GRANT ALL ON TABLE public.book_types TO postgres;


-- public.books_audit definition

-- Drop table

-- DROP TABLE public.books_audit;

CREATE TABLE public.books_audit ( audit_id int8 GENERATED ALWAYS AS IDENTITY( INCREMENT BY 1 MINVALUE 1 MAXVALUE 9223372036854775807 START 1 CACHE 1 NO CYCLE) NOT NULL, book_id int4 NOT NULL, action_type varchar(10) NOT NULL, old_data jsonb NULL, new_data jsonb NULL, action_by varchar(50) NULL, action_at timestamp DEFAULT CURRENT_TIMESTAMP NULL, CONSTRAINT books_audit_pkey PRIMARY KEY (audit_id));

-- Permissions

ALTER TABLE public.books_audit OWNER TO postgres;
GRANT ALL ON TABLE public.books_audit TO postgres;


-- public.books definition

-- Drop table

-- DROP TABLE public.books;

CREATE TABLE public.books ( id int4 GENERATED ALWAYS AS IDENTITY( INCREMENT BY 1 MINVALUE 1 MAXVALUE 2147483647 START 1 CACHE 1 NO CYCLE) NOT NULL, book_title varchar(100) NULL, author varchar(100) NULL, book_type_id int4 NULL, release_date timestamp NULL, number_of_pages int4 NULL, CONSTRAINT pk_books PRIMARY KEY (id), CONSTRAINT fk_books_book_type FOREIGN KEY (book_type_id) REFERENCES public.book_types(id) ON DELETE CASCADE);

-- Permissions

ALTER TABLE public.books OWNER TO postgres;
GRANT ALL ON TABLE public.books TO postgres;


-- DROP FUNCTION public.fn_books_audit();

CREATE OR REPLACE FUNCTION public.fn_books_audit()
 RETURNS trigger
 LANGUAGE plpgsql
AS $function$
BEGIN

IF TG_OP = 'INSERT' THEN

    INSERT INTO public.books_audit
    (book_id, action_type, new_data)
    VALUES
    (NEW.id, 'INSERT', to_jsonb(NEW));

    RETURN NEW;

ELSIF TG_OP = 'UPDATE' THEN

    INSERT INTO public.books_audit
    (book_id, action_type, old_data, new_data)
    VALUES
    (NEW.id, 'UPDATE', to_jsonb(OLD), to_jsonb(NEW));

    RETURN NEW;

ELSIF TG_OP = 'DELETE' THEN

    INSERT INTO public.books_audit
    (book_id, action_type, old_data)
    VALUES
    (OLD.id, 'DELETE', to_jsonb(OLD));

    RETURN OLD;

END IF;

RETURN NULL;

END;
$function$
;

CREATE TRIGGER trg_books_audit
AFTER INSERT OR UPDATE OR DELETE
ON public.books
FOR EACH ROW
EXECUTE FUNCTION public.fn_books_audit();