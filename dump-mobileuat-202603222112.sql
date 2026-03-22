--
-- PostgreSQL database dump
--

\restrict kB9B3aAaok0FqrxcKb70yZ0orW1ixbJEVFcT4bR1aLr0Up8BW3wt5KNTcx1NsVE

-- Dumped from database version 18.3
-- Dumped by pg_dump version 18.3

-- Started on 2026-03-22 21:12:23 IST

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET transaction_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- TOC entry 219 (class 1259 OID 18441)
-- Name: cm_about_us; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.cm_about_us (
    id integer NOT NULL,
    heading character varying(1000),
    context text
);


ALTER TABLE public.cm_about_us OWNER TO postgres;

--
-- TOC entry 220 (class 1259 OID 18447)
-- Name: cm_contact_us; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.cm_contact_us (
    id bigint NOT NULL,
    userid integer,
    message_contact text,
    createdon timestamp without time zone DEFAULT ('now'::text)::timestamp(0) with time zone
);


ALTER TABLE public.cm_contact_us OWNER TO postgres;

--
-- TOC entry 221 (class 1259 OID 18454)
-- Name: cm_contact_us_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.cm_contact_us_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.cm_contact_us_id_seq OWNER TO postgres;

--
-- TOC entry 4690 (class 0 OID 0)
-- Dependencies: 221
-- Name: cm_contact_us_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.cm_contact_us_id_seq OWNED BY public.cm_contact_us.id;


--
-- TOC entry 222 (class 1259 OID 18455)
-- Name: cm_customer_call_log_details; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.cm_customer_call_log_details (
    detailid bigint NOT NULL,
    summaryid integer,
    contactno character varying(50),
    contact_type integer,
    call_duration integer
);


ALTER TABLE public.cm_customer_call_log_details OWNER TO postgres;

--
-- TOC entry 223 (class 1259 OID 18459)
-- Name: cm_customer_call_log_details_detailid_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.cm_customer_call_log_details_detailid_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.cm_customer_call_log_details_detailid_seq OWNER TO postgres;

--
-- TOC entry 4691 (class 0 OID 0)
-- Dependencies: 223
-- Name: cm_customer_call_log_details_detailid_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.cm_customer_call_log_details_detailid_seq OWNED BY public.cm_customer_call_log_details.detailid;


--
-- TOC entry 224 (class 1259 OID 18460)
-- Name: cm_customer_call_log_summary; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.cm_customer_call_log_summary (
    summaryid bigint NOT NULL,
    customerid integer,
    last_sync_date timestamp without time zone,
    log_count integer
);


ALTER TABLE public.cm_customer_call_log_summary OWNER TO postgres;

--
-- TOC entry 225 (class 1259 OID 18464)
-- Name: cm_customer_call_log_summary_summaryid_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.cm_customer_call_log_summary_summaryid_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.cm_customer_call_log_summary_summaryid_seq OWNER TO postgres;

--
-- TOC entry 4692 (class 0 OID 0)
-- Dependencies: 225
-- Name: cm_customer_call_log_summary_summaryid_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.cm_customer_call_log_summary_summaryid_seq OWNED BY public.cm_customer_call_log_summary.summaryid;


--
-- TOC entry 226 (class 1259 OID 18465)
-- Name: cm_customer_contact_details; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.cm_customer_contact_details (
    contactid bigint NOT NULL,
    customerid integer,
    contactpersonname character varying(100),
    contactno character varying(50)
);


ALTER TABLE public.cm_customer_contact_details OWNER TO postgres;

--
-- TOC entry 227 (class 1259 OID 18469)
-- Name: cm_customer_contact_details_contactid_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.cm_customer_contact_details_contactid_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.cm_customer_contact_details_contactid_seq OWNER TO postgres;

--
-- TOC entry 4693 (class 0 OID 0)
-- Dependencies: 227
-- Name: cm_customer_contact_details_contactid_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.cm_customer_contact_details_contactid_seq OWNED BY public.cm_customer_contact_details.contactid;


--
-- TOC entry 228 (class 1259 OID 18470)
-- Name: cm_customer_fb_details; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.cm_customer_fb_details (
    id bigint NOT NULL,
    customerid integer,
    customername character varying(100),
    emailid character varying(100),
    origin character varying(100),
    profile_image text,
    dateofbirth character varying(100)
);


ALTER TABLE public.cm_customer_fb_details OWNER TO postgres;

--
-- TOC entry 229 (class 1259 OID 18476)
-- Name: cm_customer_fb_details_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.cm_customer_fb_details_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.cm_customer_fb_details_id_seq OWNER TO postgres;

--
-- TOC entry 4694 (class 0 OID 0)
-- Dependencies: 229
-- Name: cm_customer_fb_details_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.cm_customer_fb_details_id_seq OWNED BY public.cm_customer_fb_details.id;


--
-- TOC entry 230 (class 1259 OID 18477)
-- Name: cm_customer_imei; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.cm_customer_imei (
    id bigint NOT NULL,
    customerid integer,
    imei_no character varying(100),
    brandname character varying(100),
    modelname character varying(100),
    noofimages bigint,
    noofsms bigint,
    isactive boolean DEFAULT false
);


ALTER TABLE public.cm_customer_imei OWNER TO postgres;

--
-- TOC entry 231 (class 1259 OID 18482)
-- Name: cm_customer_imei_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.cm_customer_imei_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.cm_customer_imei_id_seq OWNER TO postgres;

--
-- TOC entry 4695 (class 0 OID 0)
-- Dependencies: 231
-- Name: cm_customer_imei_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.cm_customer_imei_id_seq OWNED BY public.cm_customer_imei.id;


--
-- TOC entry 232 (class 1259 OID 18483)
-- Name: cm_customer_loan_calc; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.cm_customer_loan_calc (
    id bigint NOT NULL,
    customerid integer,
    loan_amount numeric(18,2) DEFAULT 0.00,
    termtype character varying(10),
    tenure integer,
    loandate character varying(100)
);


ALTER TABLE public.cm_customer_loan_calc OWNER TO postgres;

--
-- TOC entry 233 (class 1259 OID 18488)
-- Name: cm_customer_loan_calc_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.cm_customer_loan_calc_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.cm_customer_loan_calc_id_seq OWNER TO postgres;

--
-- TOC entry 4696 (class 0 OID 0)
-- Dependencies: 233
-- Name: cm_customer_loan_calc_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.cm_customer_loan_calc_id_seq OWNED BY public.cm_customer_loan_calc.id;


--
-- TOC entry 234 (class 1259 OID 18489)
-- Name: cm_customer_location; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.cm_customer_location (
    locationid bigint NOT NULL,
    customerid integer,
    locationon timestamp without time zone DEFAULT now(),
    coordinates character varying(100)
);


ALTER TABLE public.cm_customer_location OWNER TO postgres;

--
-- TOC entry 235 (class 1259 OID 18494)
-- Name: cm_customer_location_locationid_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.cm_customer_location_locationid_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.cm_customer_location_locationid_seq OWNER TO postgres;

--
-- TOC entry 4697 (class 0 OID 0)
-- Dependencies: 235
-- Name: cm_customer_location_locationid_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.cm_customer_location_locationid_seq OWNED BY public.cm_customer_location.locationid;


--
-- TOC entry 236 (class 1259 OID 18495)
-- Name: cm_customer_notification; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.cm_customer_notification (
    id bigint NOT NULL,
    historytype character varying(100),
    amount numeric(18,0),
    historydate timestamp without time zone,
    historymsg character varying(5000),
    referenceno character varying(100),
    userid bigint,
    type character varying(100),
    for_notification boolean DEFAULT true,
    partnername character varying(100) DEFAULT ''::character varying
);


ALTER TABLE public.cm_customer_notification OWNER TO postgres;

--
-- TOC entry 237 (class 1259 OID 18503)
-- Name: cm_customer_notification_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.cm_customer_notification_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.cm_customer_notification_id_seq OWNER TO postgres;

--
-- TOC entry 4698 (class 0 OID 0)
-- Dependencies: 237
-- Name: cm_customer_notification_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.cm_customer_notification_id_seq OWNED BY public.cm_customer_notification.id;


--
-- TOC entry 238 (class 1259 OID 18504)
-- Name: cm_faq; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.cm_faq (
    id integer NOT NULL,
    question character varying(100),
    answer text,
    section character varying(100)
);


ALTER TABLE public.cm_faq OWNER TO postgres;

--
-- TOC entry 239 (class 1259 OID 18510)
-- Name: cm_jumio_retry; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.cm_jumio_retry (
    id bigint NOT NULL,
    jsondata text NOT NULL,
    jumio_ref character varying(255) NOT NULL
);


ALTER TABLE public.cm_jumio_retry OWNER TO postgres;

--
-- TOC entry 240 (class 1259 OID 18518)
-- Name: cm_jumio_retry_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.cm_jumio_retry_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.cm_jumio_retry_id_seq OWNER TO postgres;

--
-- TOC entry 4699 (class 0 OID 0)
-- Dependencies: 240
-- Name: cm_jumio_retry_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.cm_jumio_retry_id_seq OWNED BY public.cm_jumio_retry.id;


--
-- TOC entry 241 (class 1259 OID 18519)
-- Name: cm_notification_history; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.cm_notification_history (
    notify_id bigint NOT NULL,
    applicationno integer,
    contactno character varying(15),
    notificationtoken character varying(1000),
    message_text text,
    sendby text,
    sendfrom text,
    sendon timestamp without time zone DEFAULT now() NOT NULL
);


ALTER TABLE public.cm_notification_history OWNER TO postgres;

--
-- TOC entry 242 (class 1259 OID 18527)
-- Name: cm_notification_history_notify_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.cm_notification_history_notify_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.cm_notification_history_notify_id_seq OWNER TO postgres;

--
-- TOC entry 4700 (class 0 OID 0)
-- Dependencies: 242
-- Name: cm_notification_history_notify_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.cm_notification_history_notify_id_seq OWNED BY public.cm_notification_history.notify_id;


--
-- TOC entry 243 (class 1259 OID 18528)
-- Name: cm_privacy_policy; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.cm_privacy_policy (
    id integer NOT NULL,
    content text,
    contenttype character varying(10)
);


ALTER TABLE public.cm_privacy_policy OWNER TO postgres;

--
-- TOC entry 244 (class 1259 OID 18534)
-- Name: cm_rating; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.cm_rating (
    id bigint NOT NULL,
    userid integer NOT NULL,
    rating integer NOT NULL,
    optiontext text NOT NULL,
    rateon timestamp without time zone DEFAULT ('now'::text)::timestamp(0) with time zone NOT NULL
);


ALTER TABLE public.cm_rating OWNER TO postgres;

--
-- TOC entry 245 (class 1259 OID 18545)
-- Name: cm_rating_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.cm_rating_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.cm_rating_id_seq OWNER TO postgres;

--
-- TOC entry 4701 (class 0 OID 0)
-- Dependencies: 245
-- Name: cm_rating_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.cm_rating_id_seq OWNED BY public.cm_rating.id;


--
-- TOC entry 246 (class 1259 OID 18546)
-- Name: cm_reloan_notification_details; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.cm_reloan_notification_details (
    id bigint NOT NULL,
    applicationno integer NOT NULL,
    reloan_json text NOT NULL,
    notification_on timestamp without time zone DEFAULT ('now'::text)::timestamp(0) with time zone NOT NULL,
    notifyby integer NOT NULL
);


ALTER TABLE public.cm_reloan_notification_details OWNER TO postgres;

--
-- TOC entry 247 (class 1259 OID 18557)
-- Name: cm_reloan_notification_details_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.cm_reloan_notification_details_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.cm_reloan_notification_details_id_seq OWNER TO postgres;

--
-- TOC entry 4702 (class 0 OID 0)
-- Dependencies: 247
-- Name: cm_reloan_notification_details_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.cm_reloan_notification_details_id_seq OWNED BY public.cm_reloan_notification_details.id;


--
-- TOC entry 248 (class 1259 OID 18558)
-- Name: cm_tongdun_data; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.cm_tongdun_data (
    id bigint NOT NULL,
    order_no character varying(2000),
    date character varying(1000),
    country character varying(1000),
    distance character varying(1000),
    city character varying(1000),
    evaluation character varying(2000),
    car_driver character varying(1000),
    from_longitude character varying(1000),
    from_ character varying(1000),
    pay_type character varying(1000),
    tag character varying(1000),
    phone_type character varying(1000),
    vehicle_type character varying(1000),
    to_detail character varying(1000),
    currency_unit character varying(1000),
    from_latitude character varying(1000),
    to_longitude character varying(1000),
    car_no character varying(1000),
    intermediate character varying(1000),
    order_fee character varying(1000),
    to_latitude character varying(1000),
    to_ character varying(1000),
    time_ character varying(1000),
    from_detail character varying(1000),
    status character varying(1000),
    createdon timestamp without time zone DEFAULT ('now'::text)::timestamp(0) with time zone,
    userid character varying(255)
);


ALTER TABLE public.cm_tongdun_data OWNER TO postgres;

--
-- TOC entry 249 (class 1259 OID 18565)
-- Name: cm_tongdun_data_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.cm_tongdun_data_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.cm_tongdun_data_id_seq OWNER TO postgres;

--
-- TOC entry 4703 (class 0 OID 0)
-- Dependencies: 249
-- Name: cm_tongdun_data_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.cm_tongdun_data_id_seq OWNED BY public.cm_tongdun_data.id;


--
-- TOC entry 250 (class 1259 OID 18566)
-- Name: cm_tongdun_lazada; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.cm_tongdun_lazada (
    id bigint NOT NULL,
    order_no character varying(255),
    shipping_cost character varying(255),
    sub_total character varying(255),
    billing_address character varying(5000),
    billing_address_phone character varying(255),
    billing_address_name character varying(255),
    order_time character varying(255),
    grand_total character varying(255),
    shipping_address character varying(5000),
    shipping_address_name character varying(255),
    shipping_address_phone character varying(255),
    createdon timestamp without time zone DEFAULT now(),
    jumio_reference character varying(255)
);


ALTER TABLE public.cm_tongdun_lazada OWNER TO postgres;

--
-- TOC entry 251 (class 1259 OID 18573)
-- Name: cm_tongdun_lazada_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.cm_tongdun_lazada_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.cm_tongdun_lazada_id_seq OWNER TO postgres;

--
-- TOC entry 4704 (class 0 OID 0)
-- Dependencies: 251
-- Name: cm_tongdun_lazada_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.cm_tongdun_lazada_id_seq OWNED BY public.cm_tongdun_lazada.id;


--
-- TOC entry 252 (class 1259 OID 18574)
-- Name: cm_tongdun_shopee; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.cm_tongdun_shopee (
    id bigint NOT NULL,
    order_no character varying(255),
    package_shipping character varying(255),
    package_sold_by character varying(255),
    shipping_cost character varying(255),
    payment_time character varying(255),
    sub_total character varying(255),
    product_name character varying(255),
    grand_total character varying(255),
    shipping_address text,
    phone character varying(100),
    customername character varying(255),
    jumio_reference character varying(255) NOT NULL,
    createdon timestamp without time zone DEFAULT now()
);


ALTER TABLE public.cm_tongdun_shopee OWNER TO postgres;

--
-- TOC entry 253 (class 1259 OID 18582)
-- Name: cm_tongdun_shopee_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.cm_tongdun_shopee_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.cm_tongdun_shopee_id_seq OWNER TO postgres;

--
-- TOC entry 4705 (class 0 OID 0)
-- Dependencies: 253
-- Name: cm_tongdun_shopee_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.cm_tongdun_shopee_id_seq OWNED BY public.cm_tongdun_shopee.id;


--
-- TOC entry 254 (class 1259 OID 18583)
-- Name: cm_tongdun_summary; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.cm_tongdun_summary (
    id bigint NOT NULL,
    userid integer,
    summary text
);


ALTER TABLE public.cm_tongdun_summary OWNER TO postgres;

--
-- TOC entry 255 (class 1259 OID 18589)
-- Name: cm_tongdun_summary_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.cm_tongdun_summary_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.cm_tongdun_summary_id_seq OWNER TO postgres;

--
-- TOC entry 4706 (class 0 OID 0)
-- Dependencies: 255
-- Name: cm_tongdun_summary_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.cm_tongdun_summary_id_seq OWNED BY public.cm_tongdun_summary.id;


--
-- TOC entry 256 (class 1259 OID 18590)
-- Name: cm_zoloz_mapping; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.cm_zoloz_mapping (
    id bigint NOT NULL,
    zolozid character varying(255),
    transcationid character varying(255)
);


ALTER TABLE public.cm_zoloz_mapping OWNER TO postgres;

--
-- TOC entry 257 (class 1259 OID 18596)
-- Name: cm_zoloz_mapping_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.cm_zoloz_mapping_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.cm_zoloz_mapping_id_seq OWNER TO postgres;

--
-- TOC entry 4707 (class 0 OID 0)
-- Dependencies: 257
-- Name: cm_zoloz_mapping_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.cm_zoloz_mapping_id_seq OWNED BY public.cm_zoloz_mapping.id;


--
-- TOC entry 258 (class 1259 OID 19713)
-- Name: jumiodata; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.jumiodata (
    id bigint NOT NULL,
    idscanimageface character varying(255),
    idlastname character varying(255),
    idchecksecurityfeatures character varying(255),
    idchecksignature character varying(255),
    idcountry character varying(255),
    iddob character varying(255),
    idexpiry character varying(255),
    idusstate character varying(255),
    idnumber character varying(255),
    idcheckdatapositions character varying(255),
    callbacktype character varying(255),
    idcheckmicropoint character varying(255),
    idcheckdocumentvalidation character varying(255),
    idcheckhologram character varying(255),
    issuingdate character varying(255),
    idscansource character varying(255),
    idscanimagebackside character varying(255),
    idtype character varying(255),
    jumioidscanreference character varying(255),
    idscanimage character varying(255),
    idscanstatus character varying(255),
    verificationstatus character varying(255),
    transactiondate character varying(255),
    rejectreason character varying(255),
    livenessimages text,
    createdon timestamp without time zone DEFAULT now()
);


ALTER TABLE public.jumiodata OWNER TO postgres;

--
-- TOC entry 259 (class 1259 OID 19720)
-- Name: jumiodata_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.jumiodata_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.jumiodata_id_seq OWNER TO postgres;

--
-- TOC entry 4708 (class 0 OID 0)
-- Dependencies: 259
-- Name: jumiodata_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.jumiodata_id_seq OWNED BY public.jumiodata.id;


--
-- TOC entry 260 (class 1259 OID 19721)
-- Name: tbl_application_contract_details; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.tbl_application_contract_details (
    contract_no integer NOT NULL,
    application_no integer,
    contract_ref_no character varying(5000),
    sendby character varying(500),
    sendon timestamp without time zone,
    send_email character varying(500)
);


ALTER TABLE public.tbl_application_contract_details OWNER TO postgres;

--
-- TOC entry 261 (class 1259 OID 19727)
-- Name: tbl_application_contract_details_contract_no_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.tbl_application_contract_details_contract_no_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.tbl_application_contract_details_contract_no_seq OWNER TO postgres;

--
-- TOC entry 4709 (class 0 OID 0)
-- Dependencies: 261
-- Name: tbl_application_contract_details_contract_no_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.tbl_application_contract_details_contract_no_seq OWNED BY public.tbl_application_contract_details.contract_no;


--
-- TOC entry 262 (class 1259 OID 19728)
-- Name: tbl_application_defaulter_mapping; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.tbl_application_defaulter_mapping (
    id integer NOT NULL,
    application_no integer,
    defaulter_userid integer,
    created_by integer,
    updated_by integer,
    created_on timestamp without time zone DEFAULT now() NOT NULL,
    isactive boolean DEFAULT true NOT NULL,
    updated_on timestamp without time zone DEFAULT now() NOT NULL
);


ALTER TABLE public.tbl_application_defaulter_mapping OWNER TO postgres;

--
-- TOC entry 263 (class 1259 OID 19738)
-- Name: tbl_application_defaulter_mapping_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.tbl_application_defaulter_mapping_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.tbl_application_defaulter_mapping_id_seq OWNER TO postgres;

--
-- TOC entry 4710 (class 0 OID 0)
-- Dependencies: 263
-- Name: tbl_application_defaulter_mapping_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.tbl_application_defaulter_mapping_id_seq OWNED BY public.tbl_application_defaulter_mapping.id;


--
-- TOC entry 264 (class 1259 OID 19739)
-- Name: tbl_bank_master; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.tbl_bank_master (
    bank_id integer NOT NULL,
    bank_code character varying(255),
    bank_name character varying(255),
    is_active boolean DEFAULT true NOT NULL,
    created_on timestamp without time zone DEFAULT now() NOT NULL,
    updated_on timestamp without time zone DEFAULT now() NOT NULL
);


ALTER TABLE public.tbl_bank_master OWNER TO postgres;

--
-- TOC entry 265 (class 1259 OID 19751)
-- Name: tbl_bank_master_bank_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.tbl_bank_master_bank_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.tbl_bank_master_bank_id_seq OWNER TO postgres;

--
-- TOC entry 4711 (class 0 OID 0)
-- Dependencies: 265
-- Name: tbl_bank_master_bank_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.tbl_bank_master_bank_id_seq OWNED BY public.tbl_bank_master.bank_id;


--
-- TOC entry 266 (class 1259 OID 19752)
-- Name: tbl_deferment_pay; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.tbl_deferment_pay (
    deferment_id integer NOT NULL,
    emi_id integer,
    deferment_date timestamp without time zone DEFAULT now() NOT NULL,
    late_penalty_amount numeric(12,2) DEFAULT 0.00 NOT NULL,
    deferment_amount numeric(12,2) DEFAULT 0.00 NOT NULL,
    payment_channel character varying(500),
    proof_of_payment character varying(500),
    payment_approved boolean DEFAULT false NOT NULL,
    is_deleted boolean DEFAULT false NOT NULL,
    created_by integer,
    updated_by integer,
    created_on timestamp without time zone DEFAULT now() NOT NULL,
    updated_on timestamp without time zone DEFAULT now() NOT NULL
);


ALTER TABLE public.tbl_deferment_pay OWNER TO postgres;

--
-- TOC entry 267 (class 1259 OID 19772)
-- Name: tbl_deferment_pay_deferment_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.tbl_deferment_pay_deferment_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.tbl_deferment_pay_deferment_id_seq OWNER TO postgres;

--
-- TOC entry 4712 (class 0 OID 0)
-- Dependencies: 267
-- Name: tbl_deferment_pay_deferment_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.tbl_deferment_pay_deferment_id_seq OWNED BY public.tbl_deferment_pay.deferment_id;


--
-- TOC entry 268 (class 1259 OID 19773)
-- Name: tbl_email_template; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.tbl_email_template (
    id integer NOT NULL,
    email_template_name character varying(255),
    email_description character varying(255),
    email_template_definition character varying(255),
    created_on timestamp without time zone DEFAULT now() NOT NULL,
    updated_on time without time zone DEFAULT now() NOT NULL,
    isactive boolean DEFAULT true NOT NULL
);


ALTER TABLE public.tbl_email_template OWNER TO postgres;

--
-- TOC entry 269 (class 1259 OID 19785)
-- Name: tbl_email_template_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.tbl_email_template_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.tbl_email_template_id_seq OWNER TO postgres;

--
-- TOC entry 4713 (class 0 OID 0)
-- Dependencies: 269
-- Name: tbl_email_template_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.tbl_email_template_id_seq OWNED BY public.tbl_email_template.id;


--
-- TOC entry 270 (class 1259 OID 19786)
-- Name: tbl_lod; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.tbl_lod (
    id integer NOT NULL,
    application_no integer,
    lod_path character varying,
    created_on timestamp without time zone DEFAULT now() NOT NULL
);


ALTER TABLE public.tbl_lod OWNER TO postgres;

--
-- TOC entry 271 (class 1259 OID 19794)
-- Name: tbl_lod_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.tbl_lod_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.tbl_lod_id_seq OWNER TO postgres;

--
-- TOC entry 4714 (class 0 OID 0)
-- Dependencies: 271
-- Name: tbl_lod_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.tbl_lod_id_seq OWNED BY public.tbl_lod.id;


--
-- TOC entry 272 (class 1259 OID 19795)
-- Name: tbl_new_reference_approval; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.tbl_new_reference_approval (
    id integer NOT NULL,
    application_no integer,
    old_reference_no character varying(5000),
    new_reference_no character varying(5000),
    approved_by integer,
    approved_on timestamp without time zone,
    edited_by integer
);


ALTER TABLE public.tbl_new_reference_approval OWNER TO postgres;

--
-- TOC entry 273 (class 1259 OID 19801)
-- Name: tbl_new_reference_approval_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.tbl_new_reference_approval_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.tbl_new_reference_approval_id_seq OWNER TO postgres;

--
-- TOC entry 4715 (class 0 OID 0)
-- Dependencies: 273
-- Name: tbl_new_reference_approval_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.tbl_new_reference_approval_id_seq OWNED BY public.tbl_new_reference_approval.id;


--
-- TOC entry 274 (class 1259 OID 19802)
-- Name: tbl_occupation; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.tbl_occupation (
    id integer NOT NULL,
    occupation_name character varying(500),
    isactive boolean DEFAULT true NOT NULL,
    created_on timestamp without time zone DEFAULT now() NOT NULL,
    updated_on time without time zone DEFAULT now() NOT NULL,
    is_orr boolean,
    group_name character varying(100)
);


ALTER TABLE public.tbl_occupation OWNER TO postgres;

--
-- TOC entry 275 (class 1259 OID 19814)
-- Name: tbl_occupation_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.tbl_occupation_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.tbl_occupation_id_seq OWNER TO postgres;

--
-- TOC entry 4716 (class 0 OID 0)
-- Dependencies: 275
-- Name: tbl_occupation_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.tbl_occupation_id_seq OWNED BY public.tbl_occupation.id;


--
-- TOC entry 276 (class 1259 OID 19815)
-- Name: tbl_olddata; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.tbl_olddata (
    id bigint NOT NULL,
    date_applied date NOT NULL,
    applicationno character varying(255) NOT NULL,
    contractno character varying(255) NOT NULL,
    payment_ref character varying(255) NOT NULL,
    customername character varying(255) NOT NULL,
    customeremail character varying(255) NOT NULL,
    disburse_date character varying(255) NOT NULL,
    due_date1 character varying(255) DEFAULT ' '::character varying NOT NULL,
    due_date2 character varying(255) DEFAULT ' '::character varying NOT NULL,
    due_date3 character varying(255) DEFAULT ' '::character varying NOT NULL,
    due_date4 character varying(255) DEFAULT ' '::character varying NOT NULL,
    due_date5 character varying(255) DEFAULT ' '::character varying NOT NULL,
    due_date6 character varying(255) DEFAULT ' '::character varying NOT NULL,
    due_date7 character varying(255) DEFAULT ' '::character varying NOT NULL,
    due_date8 character varying(255) DEFAULT ' '::character varying NOT NULL,
    due_date9 character varying(255) DEFAULT ' '::character varying NOT NULL,
    loan_amount numeric(18,2) DEFAULT 0.00 NOT NULL,
    term_name character varying(255),
    outstanding numeric(18,2) DEFAULT 0.00,
    ammortization_payment numeric(18,2) DEFAULT 0.00,
    paid_date1 character varying(255) DEFAULT ' '::character varying NOT NULL,
    paid_date2 character varying(255) DEFAULT ' '::character varying NOT NULL,
    paid_date3 character varying(255) DEFAULT ' '::character varying NOT NULL,
    paid_date4 character varying(255) DEFAULT ' '::character varying NOT NULL,
    paid_date5 character varying(255) DEFAULT ' '::character varying NOT NULL,
    paid_date6 character varying(255) DEFAULT ' '::character varying NOT NULL,
    paid_date7 character varying(255) DEFAULT ' '::character varying NOT NULL,
    paid_date8 character varying(255) DEFAULT ' '::character varying NOT NULL,
    paid_date9 character varying(255) DEFAULT ' '::character varying NOT NULL,
    paid_amount1 numeric(18,2) DEFAULT 0.00,
    paid_amount2 numeric(18,2) DEFAULT 0.00,
    paid_amount3 numeric(18,2) DEFAULT 0.00,
    paid_amount4 numeric(18,2) DEFAULT 0.00,
    paid_amount5 numeric(18,2) DEFAULT 0.00,
    paid_amount6 numeric(18,2) DEFAULT 0.00,
    paid_amount7 numeric(18,2) DEFAULT 0.00,
    paid_amount8 numeric(18,2) DEFAULT 0.00,
    paid_amount9 numeric(18,2) DEFAULT 0.00,
    late_payment1 numeric(18,2) DEFAULT 0.00,
    late_payment2 numeric(18,2) DEFAULT 0.00,
    late_payment3 numeric(18,2) DEFAULT 0.00,
    late_payment4 numeric(18,2) DEFAULT 0.00,
    late_payment5 numeric(18,2) DEFAULT 0.00,
    late_payment6 numeric(18,2) DEFAULT 0.00,
    late_payment7 numeric(18,2) DEFAULT 0.00,
    late_payment8 numeric(18,2) DEFAULT 0.00,
    late_payment9 numeric(18,2) DEFAULT 0.00,
    penalty1 numeric(18,2) DEFAULT 0.00,
    penalty2 numeric(18,2) DEFAULT 0.00,
    penalty3 numeric(18,2) DEFAULT 0.00,
    penalty4 numeric(18,2) DEFAULT 0.00,
    penalty5 numeric(18,2) DEFAULT 0.00,
    penalty6 numeric(18,2) DEFAULT 0.00,
    penalty7 numeric(18,2) DEFAULT 0.00,
    penalty8 numeric(18,2) DEFAULT 0.00,
    penalty9 numeric(18,2) DEFAULT 0.00,
    remain_balance numeric(18,2) DEFAULT 0.00,
    total_paid numeric(18,2) DEFAULT 0.00,
    account_status character varying(255) DEFAULT ' '::character varying NOT NULL,
    remarks text
);


ALTER TABLE public.tbl_olddata OWNER TO postgres;

--
-- TOC entry 277 (class 1259 OID 19899)
-- Name: tbl_olddata_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.tbl_olddata_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.tbl_olddata_id_seq OWNER TO postgres;

--
-- TOC entry 4717 (class 0 OID 0)
-- Dependencies: 277
-- Name: tbl_olddata_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.tbl_olddata_id_seq OWNED BY public.tbl_olddata.id;


--
-- TOC entry 278 (class 1259 OID 19900)
-- Name: tbl_payment_channel; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.tbl_payment_channel (
    id integer NOT NULL,
    payment_channel character varying,
    created_on timestamp without time zone DEFAULT now() NOT NULL,
    updated_on time without time zone DEFAULT now() NOT NULL,
    isactive boolean DEFAULT true NOT NULL,
    created_by integer,
    updated_by integer
);


ALTER TABLE public.tbl_payment_channel OWNER TO postgres;

--
-- TOC entry 279 (class 1259 OID 19912)
-- Name: tbl_payment_channel_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.tbl_payment_channel_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.tbl_payment_channel_id_seq OWNER TO postgres;

--
-- TOC entry 4718 (class 0 OID 0)
-- Dependencies: 279
-- Name: tbl_payment_channel_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.tbl_payment_channel_id_seq OWNED BY public.tbl_payment_channel.id;


--
-- TOC entry 280 (class 1259 OID 19913)
-- Name: tbl_proof_of_payment; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.tbl_proof_of_payment (
    id integer NOT NULL,
    proof_of_payment character varying,
    created_on timestamp without time zone DEFAULT now() NOT NULL,
    updated_on time without time zone DEFAULT now() NOT NULL,
    isactive boolean DEFAULT true NOT NULL,
    created_by integer,
    updated_by integer
);


ALTER TABLE public.tbl_proof_of_payment OWNER TO postgres;

--
-- TOC entry 281 (class 1259 OID 19925)
-- Name: tbl_proof_of_payment_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.tbl_proof_of_payment_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.tbl_proof_of_payment_id_seq OWNER TO postgres;

--
-- TOC entry 4719 (class 0 OID 0)
-- Dependencies: 281
-- Name: tbl_proof_of_payment_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.tbl_proof_of_payment_id_seq OWNED BY public.tbl_proof_of_payment.id;


--
-- TOC entry 282 (class 1259 OID 19926)
-- Name: tbl_reason_of_nonpayment; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.tbl_reason_of_nonpayment (
    reason_id integer NOT NULL,
    reason_text text,
    isactive boolean DEFAULT true NOT NULL,
    created_on timestamp without time zone DEFAULT now() NOT NULL,
    updated_on timestamp without time zone DEFAULT now() NOT NULL
);


ALTER TABLE public.tbl_reason_of_nonpayment OWNER TO postgres;

--
-- TOC entry 283 (class 1259 OID 19938)
-- Name: tbl_reason_of_nonpayment_reason_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.tbl_reason_of_nonpayment_reason_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.tbl_reason_of_nonpayment_reason_id_seq OWNER TO postgres;

--
-- TOC entry 4720 (class 0 OID 0)
-- Dependencies: 283
-- Name: tbl_reason_of_nonpayment_reason_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.tbl_reason_of_nonpayment_reason_id_seq OWNED BY public.tbl_reason_of_nonpayment.reason_id;


--
-- TOC entry 284 (class 1259 OID 19939)
-- Name: tbl_remarks; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.tbl_remarks (
    createdon timestamp without time zone DEFAULT now() NOT NULL,
    createdby integer NOT NULL,
    createdbyname text,
    id integer NOT NULL,
    application_id integer NOT NULL,
    remark text,
    remarkidentifier character varying(255)
);


ALTER TABLE public.tbl_remarks OWNER TO postgres;

--
-- TOC entry 285 (class 1259 OID 19949)
-- Name: tbl_remarks_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.tbl_remarks_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.tbl_remarks_id_seq OWNER TO postgres;

--
-- TOC entry 4721 (class 0 OID 0)
-- Dependencies: 285
-- Name: tbl_remarks_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.tbl_remarks_id_seq OWNED BY public.tbl_remarks.id;


--
-- TOC entry 286 (class 1259 OID 19950)
-- Name: tbl_sms_template; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.tbl_sms_template (
    id integer NOT NULL,
    sms_template_name character varying(255),
    sms_description character varying(255),
    sms_template_definition character varying(255),
    created_on timestamp without time zone DEFAULT now() NOT NULL,
    updated_on time without time zone DEFAULT now() NOT NULL,
    isactive boolean DEFAULT true NOT NULL
);


ALTER TABLE public.tbl_sms_template OWNER TO postgres;

--
-- TOC entry 287 (class 1259 OID 19962)
-- Name: tbl_sms_template_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.tbl_sms_template_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.tbl_sms_template_id_seq OWNER TO postgres;

--
-- TOC entry 4722 (class 0 OID 0)
-- Dependencies: 287
-- Name: tbl_sms_template_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.tbl_sms_template_id_seq OWNED BY public.tbl_sms_template.id;


--
-- TOC entry 288 (class 1259 OID 19963)
-- Name: tbl_sssno; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.tbl_sssno (
    id integer NOT NULL,
    sss_no character varying(500),
    isactive boolean DEFAULT true NOT NULL,
    created_on timestamp without time zone DEFAULT now() NOT NULL,
    updated_on timestamp without time zone DEFAULT now() NOT NULL,
    is_orr boolean
);


ALTER TABLE public.tbl_sssno OWNER TO postgres;

--
-- TOC entry 289 (class 1259 OID 19975)
-- Name: tbl_sssno_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.tbl_sssno_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.tbl_sssno_id_seq OWNER TO postgres;

--
-- TOC entry 4723 (class 0 OID 0)
-- Dependencies: 289
-- Name: tbl_sssno_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.tbl_sssno_id_seq OWNED BY public.tbl_sssno.id;


--
-- TOC entry 290 (class 1259 OID 19976)
-- Name: tbl_team_agent_mapping; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.tbl_team_agent_mapping (
    id integer NOT NULL,
    tl_id integer,
    agent_id integer,
    isactive boolean DEFAULT true NOT NULL,
    created_by integer,
    updated_by integer,
    created_on timestamp without time zone DEFAULT now() NOT NULL,
    updated_on timestamp without time zone DEFAULT now() NOT NULL
);


ALTER TABLE public.tbl_team_agent_mapping OWNER TO postgres;

--
-- TOC entry 291 (class 1259 OID 19986)
-- Name: tbl_team_agent_mapping_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.tbl_team_agent_mapping_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.tbl_team_agent_mapping_id_seq OWNER TO postgres;

--
-- TOC entry 4724 (class 0 OID 0)
-- Dependencies: 291
-- Name: tbl_team_agent_mapping_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.tbl_team_agent_mapping_id_seq OWNED BY public.tbl_team_agent_mapping.id;


--
-- TOC entry 292 (class 1259 OID 19987)
-- Name: tbl_template_template; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.tbl_template_template (
    id integer NOT NULL,
    template_name character varying(255),
    template_description character varying(255),
    template_definition character varying(255),
    created_on timestamp without time zone DEFAULT now() NOT NULL,
    updated_on time without time zone DEFAULT now() NOT NULL,
    isactive boolean DEFAULT true NOT NULL
);


ALTER TABLE public.tbl_template_template OWNER TO postgres;

--
-- TOC entry 293 (class 1259 OID 19999)
-- Name: tbl_template_template_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.tbl_template_template_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.tbl_template_template_id_seq OWNER TO postgres;

--
-- TOC entry 4725 (class 0 OID 0)
-- Dependencies: 293
-- Name: tbl_template_template_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.tbl_template_template_id_seq OWNED BY public.tbl_template_template.id;


--
-- TOC entry 294 (class 1259 OID 20000)
-- Name: tbladdresshistory; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.tbladdresshistory (
    id bigint NOT NULL,
    provinceid integer,
    cityid integer,
    barangayid integer,
    address character varying(255),
    userid integer,
    createdon timestamp without time zone DEFAULT ('now'::text)::timestamp(0) with time zone
);


ALTER TABLE public.tbladdresshistory OWNER TO postgres;

--
-- TOC entry 295 (class 1259 OID 20005)
-- Name: tbladdresshistory_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.tbladdresshistory_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.tbladdresshistory_id_seq OWNER TO postgres;

--
-- TOC entry 4726 (class 0 OID 0)
-- Dependencies: 295
-- Name: tbladdresshistory_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.tbladdresshistory_id_seq OWNED BY public.tbladdresshistory.id;


--
-- TOC entry 296 (class 1259 OID 20006)
-- Name: tblapplication_emi; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.tblapplication_emi (
    emi_id integer NOT NULL,
    application_no integer,
    reference_no character varying(100),
    disbursement_date date,
    penality_amount1 numeric(10,2) DEFAULT 0 NOT NULL,
    penality_amount2 numeric(10,2) DEFAULT 0 NOT NULL,
    penality_amount3 numeric(10,2) DEFAULT 0 NOT NULL,
    penality_amount4 numeric(10,2) DEFAULT 0 NOT NULL,
    penality_amount5 numeric(10,2) DEFAULT 0 NOT NULL,
    createdon timestamp without time zone DEFAULT now() NOT NULL
);


ALTER TABLE public.tblapplication_emi OWNER TO postgres;

--
-- TOC entry 297 (class 1259 OID 20022)
-- Name: tblapplication_emi_details; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.tblapplication_emi_details (
    emi_detail_id integer NOT NULL,
    emi_id integer,
    emi_date date,
    emi_amount numeric(10,2),
    emi_rate numeric(10,2),
    emi_principal numeric(10,2),
    emi_status integer,
    createbon timestamp without time zone DEFAULT now(),
    updatedon timestamp without time zone DEFAULT now(),
    updatedby character varying(100),
    emi_weekterm character varying(50),
    paidamount numeric(10,2) DEFAULT 0.00 NOT NULL,
    balanceamount numeric(10,2) DEFAULT 0.00 NOT NULL,
    is_pay boolean DEFAULT false NOT NULL,
    is_partial boolean DEFAULT false NOT NULL
);


ALTER TABLE public.tblapplication_emi_details OWNER TO postgres;

--
-- TOC entry 298 (class 1259 OID 20036)
-- Name: tblapplication_emi_details_emi_detail_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.tblapplication_emi_details_emi_detail_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.tblapplication_emi_details_emi_detail_id_seq OWNER TO postgres;

--
-- TOC entry 4727 (class 0 OID 0)
-- Dependencies: 298
-- Name: tblapplication_emi_details_emi_detail_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.tblapplication_emi_details_emi_detail_id_seq OWNED BY public.tblapplication_emi_details.emi_detail_id;


--
-- TOC entry 299 (class 1259 OID 20037)
-- Name: tblapplication_emi_emi_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.tblapplication_emi_emi_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.tblapplication_emi_emi_id_seq OWNER TO postgres;

--
-- TOC entry 4728 (class 0 OID 0)
-- Dependencies: 299
-- Name: tblapplication_emi_emi_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.tblapplication_emi_emi_id_seq OWNED BY public.tblapplication_emi.emi_id;


--
-- TOC entry 300 (class 1259 OID 20038)
-- Name: tblapplication_emi_payment; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.tblapplication_emi_payment (
    payment_id integer NOT NULL,
    application_no integer,
    paid_amount numeric(10,2) DEFAULT 0.00,
    paid_on timestamp without time zone DEFAULT now(),
    updatedon timestamp without time zone DEFAULT now(),
    updatedby character varying(100),
    whatpaymentchennel character varying(500),
    proofofpayment character varying(500),
    paymentapproved boolean DEFAULT false NOT NULL,
    isdeleted boolean DEFAULT false NOT NULL,
    penalty_amount numeric(10,2) DEFAULT 0.00,
    remarks character varying(500),
    is_pay boolean DEFAULT false NOT NULL,
    is_partial boolean DEFAULT false NOT NULL
);


ALTER TABLE public.tblapplication_emi_payment OWNER TO postgres;

--
-- TOC entry 301 (class 1259 OID 20056)
-- Name: tblapplication_emi_payment_payment_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.tblapplication_emi_payment_payment_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.tblapplication_emi_payment_payment_id_seq OWNER TO postgres;

--
-- TOC entry 4729 (class 0 OID 0)
-- Dependencies: 301
-- Name: tblapplication_emi_payment_payment_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.tblapplication_emi_payment_payment_id_seq OWNED BY public.tblapplication_emi_payment.payment_id;


--
-- TOC entry 302 (class 1259 OID 20057)
-- Name: tblapplication_payment_proof; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.tblapplication_payment_proof (
    id bigint NOT NULL,
    applicationno integer,
    proof_of_payment text,
    uploadedon timestamp without time zone DEFAULT ('now'::text)::timestamp(0) with time zone,
    payment_partner text,
    ischecked boolean DEFAULT false,
    amount numeric(18,2) DEFAULT 0.00,
    transactionid character varying(255)
);


ALTER TABLE public.tblapplication_payment_proof OWNER TO postgres;

--
-- TOC entry 303 (class 1259 OID 20066)
-- Name: tblapplication_payment_proof_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.tblapplication_payment_proof_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.tblapplication_payment_proof_id_seq OWNER TO postgres;

--
-- TOC entry 4730 (class 0 OID 0)
-- Dependencies: 303
-- Name: tblapplication_payment_proof_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.tblapplication_payment_proof_id_seq OWNED BY public.tblapplication_payment_proof.id;


--
-- TOC entry 304 (class 1259 OID 20067)
-- Name: tblapplication_personal_verification_details; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.tblapplication_personal_verification_details (
    detailid integer NOT NULL,
    application_no integer,
    requestedamt integer,
    requestedterms integer,
    id1_status boolean,
    id2_status boolean,
    pi_status boolean,
    pb_status boolean,
    remarks character varying(5000),
    addtional_birth_place character varying(250),
    additonal_provincial_address character varying(5000),
    additional_loan_purpose character varying(5000),
    additional_requested_loan_amount numeric,
    additional_requested_term numeric,
    additional_employed_duration numeric,
    additional_job_position character varying(500),
    additional_job_level character varying(500),
    additional_paydate character varying(500),
    family_personal_name_status boolean,
    family_personal_contact_status boolean,
    family_relation_with_borrower_status boolean,
    family_borrower_known_duration_status boolean,
    family_address_verification_status boolean,
    family_borrower_working_place_status boolean,
    friend_personal_name_status boolean,
    friend_personal_contact_status boolean,
    friend_relation_with_borrower_status boolean,
    friend_borrower_known_duration_status boolean,
    friend_address_verification_status boolean,
    friend_borrower_working_place boolean,
    co_worker_personal_name_status boolean,
    co_worker_personal_contact_status boolean,
    co_worker_relation_with_borrower_status boolean,
    co_worker_borrower_known_duration_status boolean,
    co_worker_address_verification_status boolean,
    co_worker_borrower_working_place boolean,
    employement_nameof_work_contact_status boolean,
    employement_no_of_work_contact_status boolean,
    employement_borrower_working_status boolean,
    employement_borrower_position_status boolean,
    employement_borrower_monthly_salary_status boolean,
    employement_borrower_attendance_status boolean,
    employement_borrower_bank_payroll_status boolean,
    family_personal_name character varying(500),
    family_personal_contact character varying(500),
    family_relation_with_borrower character varying(500),
    family_borrower_known_duration character varying(500),
    family_address_verification character varying(500),
    family_borrower_working_place character varying(500),
    friend_personal_name character varying(500),
    friend_personal_contact character varying(500),
    friend_relation_with_borrower character varying(500),
    friend_borrower_known_duration character varying(500),
    friend_address_verification character varying(500),
    friend_borrower_working character varying(500),
    co_worker_personal_name character varying(500),
    co_worker_personal_contact character varying(500),
    co_worker_relation_with_borrower character varying(500),
    co_worker_borrower_known_duration character varying(500),
    co_worker_address_verification character varying(500),
    co_worker_borrower_working character varying(500),
    employement_nameof_work_contact character varying(500),
    employement_no_of_work_contact character varying(500),
    employement_borrower_working character varying(500) DEFAULT 0.00,
    employement_borrower_position character varying(500),
    employement_borrower_monthly_salary character varying(500) DEFAULT 0.00,
    employement_borrower_attendance character varying(500),
    employement_borrower_bank_payroll character varying(500),
    question1_status boolean,
    question2_status boolean,
    question3_status boolean,
    question4_status boolean,
    question5_status boolean,
    question6_status boolean,
    question7_status boolean,
    question8_status boolean,
    question9_status boolean,
    question10_status boolean,
    approved_loan_amount numeric,
    approved_term numeric,
    approved_interest_rate numeric,
    approved_maturity_date date,
    approved_total_amount_due numeric,
    nearest_landmark character varying(255),
    rented_mortgage_owned character varying(255),
    tansfer_residence character varying(255),
    spouse_name character varying(255),
    spouse_occupation character varying(255),
    number_of_dependent integer DEFAULT 0 CONSTRAINT tblapplication_personal_verificati_number_of_dependent_not_null NOT NULL,
    mother_work character varying(255),
    father_name character varying(255),
    father_work character varying(255),
    living_with_mother character varying(255),
    sibling_count integer DEFAULT 0 CONSTRAINT tblapplication_personal_verification_det_sibling_count_not_null NOT NULL,
    sibling_works character varying(255),
    occupation character varying(255),
    scheduled_and_btc character varying(255),
    net_income numeric DEFAULT 0 CONSTRAINT tblapplication_personal_verification_detail_net_income_not_null NOT NULL,
    pending_resignation character varying(255),
    other_source_of_income character varying(255),
    know_about_cashmart character varying(255),
    pending_loan_fron_otherland character varying(255),
    bank_loan_or_credit_card character varying(255),
    permanent_address character varying(500),
    bankid integer,
    family_name1 character varying(500),
    family_address1 character varying(500),
    family_contact1 character varying(500),
    family_relation1 character varying(500),
    family_name2 character varying(500),
    family_address2 character varying(500),
    family_contact2 character varying(500),
    family_relation2 character varying(500),
    optional_name character varying(500),
    optional_address character varying(500),
    optional_contact character varying(500),
    optional_relation character varying(500)
);


ALTER TABLE public.tblapplication_personal_verification_details OWNER TO postgres;

--
-- TOC entry 4731 (class 0 OID 0)
-- Dependencies: 304
-- Name: COLUMN tblapplication_personal_verification_details.family_name1; Type: COMMENT; Schema: public; Owner: postgres
--

COMMENT ON COLUMN public.tblapplication_personal_verification_details.family_name1 IS '
';


--
-- TOC entry 305 (class 1259 OID 20081)
-- Name: tblapplication_personal_verification_details_detailid_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.tblapplication_personal_verification_details_detailid_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.tblapplication_personal_verification_details_detailid_seq OWNER TO postgres;

--
-- TOC entry 4732 (class 0 OID 0)
-- Dependencies: 305
-- Name: tblapplication_personal_verification_details_detailid_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.tblapplication_personal_verification_details_detailid_seq OWNED BY public.tblapplication_personal_verification_details.detailid;


--
-- TOC entry 306 (class 1259 OID 20082)
-- Name: tblapplication_ptp_details; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.tblapplication_ptp_details (
    ptp_id integer NOT NULL,
    application_on integer,
    date_paid date,
    amount_paid numeric(10,2),
    payment_channel character varying(500),
    payment_proof character varying(500),
    best_time_callback character varying(500),
    current_date_ptp date,
    current_amount_ptp numeric(10,2),
    non_payment_reason character varying(5000),
    other_remarks character varying(5000),
    createdon timestamp without time zone DEFAULT now() NOT NULL,
    last_updatedby character varying(5000) NOT NULL,
    contactno character varying(50),
    penality_amount numeric(10,2)
);


ALTER TABLE public.tblapplication_ptp_details OWNER TO postgres;

--
-- TOC entry 307 (class 1259 OID 20091)
-- Name: tblapplication_ptp_details_ptp_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.tblapplication_ptp_details_ptp_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.tblapplication_ptp_details_ptp_id_seq OWNER TO postgres;

--
-- TOC entry 4733 (class 0 OID 0)
-- Dependencies: 307
-- Name: tblapplication_ptp_details_ptp_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.tblapplication_ptp_details_ptp_id_seq OWNED BY public.tblapplication_ptp_details.ptp_id;


--
-- TOC entry 308 (class 1259 OID 20092)
-- Name: tblapplication_record; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.tblapplication_record (
    applicationno integer NOT NULL,
    dateapplied timestamp without time zone DEFAULT now(),
    relativecontactno character varying(50),
    coworkercontactno character varying(50),
    address character varying(2000),
    homephoneno character varying(50),
    placeofbirth character varying(50),
    dateofbirth date,
    civilstatus integer,
    mothermaidenname character varying(50),
    motheraddress character varying(100),
    companyname character varying(100),
    companyaddress character varying(200),
    designation character varying(50),
    gross_income numeric,
    reference_name character varying(50),
    reference_contactno character varying(50),
    bankname character varying(50),
    bankaccountno character varying(50),
    city integer,
    ischeck boolean DEFAULT false,
    isverified boolean DEFAULT false,
    isrecheck boolean DEFAULT false,
    isreverified boolean DEFAULT false,
    isapproved boolean DEFAULT false,
    createdon timestamp without time zone DEFAULT now(),
    updatedon timestamp without time zone DEFAULT now(),
    relativename character varying(255),
    coworkername character varying(255),
    relationwithrelative character varying(255),
    friendname character varying(255),
    gov_id_url text,
    companyid_url text,
    billing_url text,
    income_url text,
    suffix character varying(10),
    street character varying(255),
    zipcode character varying(10),
    company_phoneno text,
    friend_contactno character varying(20),
    termaccepted boolean DEFAULT false,
    date_joining date,
    pay_date date,
    checkedon timestamp without time zone DEFAULT now(),
    recheckedon timestamp without time zone DEFAULT now(),
    verifiedon timestamp without time zone DEFAULT now(),
    reverifiedon timestamp without time zone DEFAULT now(),
    approvedon timestamp without time zone DEFAULT now(),
    loanamount numeric,
    ispickedchecker boolean DEFAULT false,
    ispickedverifier boolean DEFAULT false,
    ispickedapprover boolean DEFAULT false,
    term integer,
    other_url text,
    termtype integer,
    purposeofloan character varying(200),
    leadid integer,
    paydate1 integer,
    paydate2 integer,
    isfordisbursement boolean DEFAULT false NOT NULL,
    isdisbursedexport boolean DEFAULT false NOT NULL,
    isdeleted boolean DEFAULT false NOT NULL,
    remarks character varying(5000),
    is_picked_for_rechecker boolean DEFAULT false NOT NULL,
    ispickedreverifier boolean DEFAULT false NOT NULL,
    isinsufficient_doc boolean DEFAULT false NOT NULL,
    isdefaulter boolean DEFAULT false NOT NULL,
    checker_remark character varying(500),
    ispickedforaccount boolean DEFAULT false NOT NULL,
    ispickedforcollector boolean DEFAULT false NOT NULL,
    morning_time integer DEFAULT '-1'::integer,
    noon_time integer DEFAULT '-1'::integer,
    verify_by integer,
    checked_by integer,
    approved_by integer,
    checker_oic_on timestamp without time zone,
    verifier_oic_on timestamp without time zone,
    approver_oic_on timestamp without time zone,
    disbursement_date timestamp without time zone,
    sss_no character varying(50),
    province integer,
    industry integer,
    otp_code integer,
    user_id integer,
    atm_url text,
    is_pay boolean DEFAULT false NOT NULL,
    isfor_reduceloan boolean,
    is_ptp boolean DEFAULT false NOT NULL,
    isfor_reloan boolean DEFAULT false NOT NULL,
    is_preterm boolean DEFAULT false NOT NULL,
    reloan_date timestamp without time zone,
    preterm_date timestamp without time zone,
    resume_to_collector boolean DEFAULT false NOT NULL,
    is_agency boolean DEFAULT false NOT NULL,
    escalate_date timestamp without time zone,
    isfor_fsbucket boolean DEFAULT false NOT NULL,
    is_orr_declined boolean DEFAULT false,
    net_monthly_income numeric,
    preterm_by integer,
    reloan_by integer,
    rejectedbyverifier boolean DEFAULT false NOT NULL,
    rejectedbyapprover boolean DEFAULT false NOT NULL,
    resumetocollector_date timestamp without time zone,
    ispretermgenerated boolean DEFAULT false,
    dependent integer DEFAULT 0,
    home_status integer DEFAULT 0,
    working_schedule integer DEFAULT 0,
    job_duration integer DEFAULT 0,
    fromapp boolean DEFAULT false NOT NULL,
    gov_doc_type character varying(1000) DEFAULT ''::character varying,
    loanrequiredon timestamp without time zone,
    iscompleted boolean DEFAULT false,
    signurl text,
    completeon timestamp without time zone,
    barangay integer,
    guid character varying(100),
    gender character varying(10)
);


ALTER TABLE public.tblapplication_record OWNER TO postgres;

--
-- TOC entry 309 (class 1259 OID 20163)
-- Name: tblapplication_record_applicationno_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.tblapplication_record_applicationno_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.tblapplication_record_applicationno_seq OWNER TO postgres;

--
-- TOC entry 4734 (class 0 OID 0)
-- Dependencies: 309
-- Name: tblapplication_record_applicationno_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.tblapplication_record_applicationno_seq OWNED BY public.tblapplication_record.applicationno;


--
-- TOC entry 310 (class 1259 OID 20164)
-- Name: tblapplication_reloan_preterm_details; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.tblapplication_reloan_preterm_details (
    id bigint NOT NULL,
    oldappno integer,
    newappno integer,
    isreloan boolean,
    pretermamount numeric(18,2) DEFAULT 0.00
);


ALTER TABLE public.tblapplication_reloan_preterm_details OWNER TO postgres;

--
-- TOC entry 311 (class 1259 OID 20169)
-- Name: tblapplication_reloan_preterm_details_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.tblapplication_reloan_preterm_details_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.tblapplication_reloan_preterm_details_id_seq OWNER TO postgres;

--
-- TOC entry 4735 (class 0 OID 0)
-- Dependencies: 311
-- Name: tblapplication_reloan_preterm_details_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.tblapplication_reloan_preterm_details_id_seq OWNED BY public.tblapplication_reloan_preterm_details.id;


--
-- TOC entry 312 (class 1259 OID 20170)
-- Name: tblapplication_user; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.tblapplication_user (
    id integer NOT NULL,
    first_name character varying(200),
    middle_name character varying(200),
    last_name character varying(200),
    personalemail character varying(200),
    user_password character varying(200),
    personalcontactno character varying(50),
    lifetime_id character varying(100),
    created_on timestamp without time zone DEFAULT now() NOT NULL,
    updated_on timestamp without time zone DEFAULT now() NOT NULL,
    notificationtoken text,
    facemapname character varying(255),
    facemapstatus character varying(255),
    jumioreference text,
    ismobile boolean DEFAULT false,
    pin_code character varying(10),
    guid character varying(255)
);


ALTER TABLE public.tblapplication_user OWNER TO postgres;

--
-- TOC entry 313 (class 1259 OID 20181)
-- Name: tblapplication_user_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.tblapplication_user_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.tblapplication_user_id_seq OWNER TO postgres;

--
-- TOC entry 4736 (class 0 OID 0)
-- Dependencies: 313
-- Name: tblapplication_user_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.tblapplication_user_id_seq OWNED BY public.tblapplication_user.id;


--
-- TOC entry 314 (class 1259 OID 20182)
-- Name: tblapplicationdata_verification_details; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.tblapplicationdata_verification_details (
    verification_id integer CONSTRAINT tblapplicationdata_verification_detail_verification_id_not_null NOT NULL,
    application_no integer,
    personal_name_remark character varying(500),
    personal_email_remark character varying(500),
    personal_contact_no_remark character varying(500),
    personal_perma_address_remark character varying(500),
    personal_house_ph_remark character varying(500),
    personal_birth_place_remark character varying(500),
    personal_birth_date_remark character varying(500),
    personal_civil_remark character varying(500),
    personal_mother_maiden_name_remark character varying(500),
    personal_mother_perma_add_remark character varying(500),
    personal_company_name_remark character varying(500),
    personal_company_address_remark character varying(500),
    personal_job_title_remark character varying(500),
    personal_monthly_income_remark character varying(500),
    personal_reference_name_remark character varying(500),
    personal_reference_contact_remark character varying(500),
    personal_bank_name_remark character varying(500),
    personal_bank_ac_no_remark character varying(500),
    personal_req_loan_amt_remark character varying(500),
    personal_req_loan_term_remark character varying(500),
    nearest_landmark_remark character varying(500),
    sss_no_remark character varying(500),
    rented_mortgage_owned_remark character varying(500),
    permanent_address_remark character varying(500),
    tansfer_residence_remark character varying(500),
    spouse_name_remark character varying(500),
    spouse_occupation_remark character varying(500),
    number_of_dependent_remark character varying(500),
    father_name_work_remark character varying(500),
    sibling_works_remark character varying(500),
    occupation_remark character varying(500),
    net_income_remark character varying(500),
    scheduled_and_btc_remark character varying(500),
    pay_date_remark character varying(500),
    pending_resignation_remark character varying(500),
    other_source_of_income_remark character varying(500),
    know_about_cashmart_remark character varying(500),
    pending_loan_fron_otherland_remark character varying(500),
    bank_loan_or_credit_card_remark character varying(500),
    family_name1_remark character varying(500),
    family_address1_remark character varying(500),
    family_contact1_remark character varying(500),
    family_relation1_remark character varying(500),
    family_name2_remark character varying(500),
    family_address2_remark character varying(500),
    family_contact2_remark character varying(500),
    family_relation2_remark character varying(500),
    optional_name_remark character varying(500),
    optional_address_remark character varying(500),
    optional_contact_remark character varying(500),
    optional_relation_remark character varying(500)
);


ALTER TABLE public.tblapplicationdata_verification_details OWNER TO postgres;

--
-- TOC entry 4737 (class 0 OID 0)
-- Dependencies: 314
-- Name: COLUMN tblapplicationdata_verification_details.family_name1_remark; Type: COMMENT; Schema: public; Owner: postgres
--

COMMENT ON COLUMN public.tblapplicationdata_verification_details.family_name1_remark IS '
';


--
-- TOC entry 315 (class 1259 OID 20348)
-- Name: tblapplicationdata_verification_details_verification_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.tblapplicationdata_verification_details_verification_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.tblapplicationdata_verification_details_verification_id_seq OWNER TO postgres;

--
-- TOC entry 4738 (class 0 OID 0)
-- Dependencies: 315
-- Name: tblapplicationdata_verification_details_verification_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.tblapplicationdata_verification_details_verification_id_seq OWNED BY public.tblapplicationdata_verification_details.verification_id;


--
-- TOC entry 316 (class 1259 OID 20349)
-- Name: tblbarangay; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.tblbarangay (
    id integer NOT NULL,
    barangay_name character varying(255),
    city_id integer NOT NULL,
    is_orr boolean DEFAULT false NOT NULL,
    created_on timestamp without time zone DEFAULT now() NOT NULL,
    updated_on time without time zone DEFAULT now() NOT NULL,
    isactive boolean DEFAULT true NOT NULL
);


ALTER TABLE public.tblbarangay OWNER TO postgres;

--
-- TOC entry 317 (class 1259 OID 20362)
-- Name: tblbarangay_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.tblbarangay_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.tblbarangay_id_seq OWNER TO postgres;

--
-- TOC entry 4739 (class 0 OID 0)
-- Dependencies: 317
-- Name: tblbarangay_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.tblbarangay_id_seq OWNED BY public.tblbarangay.id;


--
-- TOC entry 318 (class 1259 OID 20363)
-- Name: tblcity; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.tblcity (
    cityid integer NOT NULL,
    cityname character varying(50),
    province_id integer,
    is_orr boolean DEFAULT false NOT NULL,
    isactive boolean DEFAULT true NOT NULL
);


ALTER TABLE public.tblcity OWNER TO postgres;

--
-- TOC entry 319 (class 1259 OID 20371)
-- Name: tblcity_cityid_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.tblcity_cityid_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.tblcity_cityid_seq OWNER TO postgres;

--
-- TOC entry 4740 (class 0 OID 0)
-- Dependencies: 319
-- Name: tblcity_cityid_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.tblcity_cityid_seq OWNED BY public.tblcity.cityid;


--
-- TOC entry 320 (class 1259 OID 20372)
-- Name: tblcivil_status; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.tblcivil_status (
    civilid integer NOT NULL,
    civilname character varying
);


ALTER TABLE public.tblcivil_status OWNER TO postgres;

--
-- TOC entry 321 (class 1259 OID 20378)
-- Name: tblcivil_status_civilid_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.tblcivil_status_civilid_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.tblcivil_status_civilid_seq OWNER TO postgres;

--
-- TOC entry 4741 (class 0 OID 0)
-- Dependencies: 321
-- Name: tblcivil_status_civilid_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.tblcivil_status_civilid_seq OWNED BY public.tblcivil_status.civilid;


--
-- TOC entry 322 (class 1259 OID 20379)
-- Name: tbldefaulter_collector_movement_history; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.tbldefaulter_collector_movement_history (
    id bigint NOT NULL,
    application_no integer,
    defaulterid integer,
    movedon timestamp without time zone DEFAULT ('now'::text)::timestamp(0) with time zone NOT NULL,
    collector_id integer
);


ALTER TABLE public.tbldefaulter_collector_movement_history OWNER TO postgres;

--
-- TOC entry 323 (class 1259 OID 20385)
-- Name: tbldefaulter_collector_movement_history_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.tbldefaulter_collector_movement_history_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.tbldefaulter_collector_movement_history_id_seq OWNER TO postgres;

--
-- TOC entry 4742 (class 0 OID 0)
-- Dependencies: 323
-- Name: tbldefaulter_collector_movement_history_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.tbldefaulter_collector_movement_history_id_seq OWNED BY public.tbldefaulter_collector_movement_history.id;


--
-- TOC entry 324 (class 1259 OID 20386)
-- Name: tbldisbursement_record; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.tbldisbursement_record (
    id integer NOT NULL,
    application_no integer,
    disbursement_amount numeric(10,2) DEFAULT 0.00 NOT NULL,
    created_by integer,
    created_on timestamp without time zone DEFAULT now() NOT NULL
);


ALTER TABLE public.tbldisbursement_record OWNER TO postgres;

--
-- TOC entry 325 (class 1259 OID 20394)
-- Name: tbldisbursement_record_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.tbldisbursement_record_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.tbldisbursement_record_id_seq OWNER TO postgres;

--
-- TOC entry 4743 (class 0 OID 0)
-- Dependencies: 325
-- Name: tbldisbursement_record_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.tbldisbursement_record_id_seq OWNED BY public.tbldisbursement_record.id;


--
-- TOC entry 326 (class 1259 OID 20395)
-- Name: tblemi_collection_agent_mapping; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.tblemi_collection_agent_mapping (
    id integer NOT NULL,
    emi_id integer,
    agent_id integer,
    assigned_date timestamp without time zone DEFAULT now() NOT NULL,
    last_updatedon timestamp without time zone DEFAULT now() NOT NULL
);


ALTER TABLE public.tblemi_collection_agent_mapping OWNER TO postgres;

--
-- TOC entry 327 (class 1259 OID 20403)
-- Name: tblemi_collection_agent_mapping_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.tblemi_collection_agent_mapping_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.tblemi_collection_agent_mapping_id_seq OWNER TO postgres;

--
-- TOC entry 4744 (class 0 OID 0)
-- Dependencies: 327
-- Name: tblemi_collection_agent_mapping_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.tblemi_collection_agent_mapping_id_seq OWNED BY public.tblemi_collection_agent_mapping.id;


--
-- TOC entry 328 (class 1259 OID 20404)
-- Name: tblindustry; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.tblindustry (
    createdon timestamp without time zone NOT NULL,
    updatedon timestamp without time zone NOT NULL,
    createdby integer NOT NULL,
    updatedby integer NOT NULL,
    createdbyname text,
    updatedbyname text,
    isactive boolean,
    id integer NOT NULL,
    industry_name character varying(255),
    is_orr boolean
);


ALTER TABLE public.tblindustry OWNER TO postgres;

--
-- TOC entry 329 (class 1259 OID 20414)
-- Name: tblindustry_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.tblindustry_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.tblindustry_id_seq OWNER TO postgres;

--
-- TOC entry 4745 (class 0 OID 0)
-- Dependencies: 329
-- Name: tblindustry_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.tblindustry_id_seq OWNED BY public.tblindustry.id;


--
-- TOC entry 330 (class 1259 OID 20415)
-- Name: tblotp; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.tblotp (
    isactive boolean DEFAULT true NOT NULL,
    id integer NOT NULL,
    contactno character varying(500),
    otp_code integer,
    created_on timestamp without time zone DEFAULT now() NOT NULL,
    updated_on timestamp without time zone DEFAULT now() NOT NULL
);


ALTER TABLE public.tblotp OWNER TO postgres;

--
-- TOC entry 331 (class 1259 OID 20427)
-- Name: tblotp_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.tblotp_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.tblotp_id_seq OWNER TO postgres;

--
-- TOC entry 4746 (class 0 OID 0)
-- Dependencies: 331
-- Name: tblotp_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.tblotp_id_seq OWNED BY public.tblotp.id;


--
-- TOC entry 332 (class 1259 OID 20428)
-- Name: tblprovince; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.tblprovince (
    province_id integer NOT NULL,
    province_name character varying(255),
    is_orr boolean DEFAULT false NOT NULL,
    created_on timestamp without time zone DEFAULT now() NOT NULL,
    updated_on time without time zone DEFAULT now() NOT NULL,
    isactive boolean DEFAULT true NOT NULL
);


ALTER TABLE public.tblprovince OWNER TO postgres;

--
-- TOC entry 333 (class 1259 OID 20440)
-- Name: tblprovince_province_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.tblprovince_province_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.tblprovince_province_id_seq OWNER TO postgres;

--
-- TOC entry 4747 (class 0 OID 0)
-- Dependencies: 333
-- Name: tblprovince_province_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.tblprovince_province_id_seq OWNED BY public.tblprovince.province_id;


--
-- TOC entry 334 (class 1259 OID 20441)
-- Name: tblterm_type; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.tblterm_type (
    id integer NOT NULL,
    termtype character varying(50)
);


ALTER TABLE public.tblterm_type OWNER TO postgres;

--
-- TOC entry 335 (class 1259 OID 20445)
-- Name: tblterm_type_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.tblterm_type_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.tblterm_type_id_seq OWNER TO postgres;

--
-- TOC entry 4748 (class 0 OID 0)
-- Dependencies: 335
-- Name: tblterm_type_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.tblterm_type_id_seq OWNED BY public.tblterm_type.id;


--
-- TOC entry 336 (class 1259 OID 20446)
-- Name: tblterms; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.tblterms (
    id integer NOT NULL,
    term_id integer,
    term_name character varying(50),
    term_value character varying(50),
    interest_rate numeric(10,2) DEFAULT 0.00 NOT NULL,
    late_rate numeric(10,4) DEFAULT 0.00 NOT NULL,
    isdeleted boolean DEFAULT false NOT NULL,
    ispublic boolean DEFAULT false NOT NULL,
    admin_fee numeric(10,2) DEFAULT 0.00 NOT NULL,
    late_penalty numeric(10,2) DEFAULT 0.00 NOT NULL,
    applicabletill date,
    upto10k boolean DEFAULT false
);


ALTER TABLE public.tblterms OWNER TO postgres;

--
-- TOC entry 337 (class 1259 OID 20463)
-- Name: tblterms_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.tblterms_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.tblterms_id_seq OWNER TO postgres;

--
-- TOC entry 4749 (class 0 OID 0)
-- Dependencies: 337
-- Name: tblterms_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.tblterms_id_seq OWNED BY public.tblterms.id;


--
-- TOC entry 3973 (class 2604 OID 20540)
-- Name: cm_contact_us id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.cm_contact_us ALTER COLUMN id SET DEFAULT nextval('public.cm_contact_us_id_seq'::regclass);


--
-- TOC entry 3975 (class 2604 OID 20541)
-- Name: cm_customer_call_log_details detailid; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.cm_customer_call_log_details ALTER COLUMN detailid SET DEFAULT nextval('public.cm_customer_call_log_details_detailid_seq'::regclass);


--
-- TOC entry 3976 (class 2604 OID 20542)
-- Name: cm_customer_call_log_summary summaryid; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.cm_customer_call_log_summary ALTER COLUMN summaryid SET DEFAULT nextval('public.cm_customer_call_log_summary_summaryid_seq'::regclass);


--
-- TOC entry 3977 (class 2604 OID 20543)
-- Name: cm_customer_contact_details contactid; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.cm_customer_contact_details ALTER COLUMN contactid SET DEFAULT nextval('public.cm_customer_contact_details_contactid_seq'::regclass);


--
-- TOC entry 3978 (class 2604 OID 20544)
-- Name: cm_customer_fb_details id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.cm_customer_fb_details ALTER COLUMN id SET DEFAULT nextval('public.cm_customer_fb_details_id_seq'::regclass);


--
-- TOC entry 3979 (class 2604 OID 20545)
-- Name: cm_customer_imei id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.cm_customer_imei ALTER COLUMN id SET DEFAULT nextval('public.cm_customer_imei_id_seq'::regclass);


--
-- TOC entry 3981 (class 2604 OID 20546)
-- Name: cm_customer_loan_calc id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.cm_customer_loan_calc ALTER COLUMN id SET DEFAULT nextval('public.cm_customer_loan_calc_id_seq'::regclass);


--
-- TOC entry 3983 (class 2604 OID 20547)
-- Name: cm_customer_location locationid; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.cm_customer_location ALTER COLUMN locationid SET DEFAULT nextval('public.cm_customer_location_locationid_seq'::regclass);


--
-- TOC entry 3985 (class 2604 OID 20548)
-- Name: cm_customer_notification id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.cm_customer_notification ALTER COLUMN id SET DEFAULT nextval('public.cm_customer_notification_id_seq'::regclass);


--
-- TOC entry 3988 (class 2604 OID 20549)
-- Name: cm_jumio_retry id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.cm_jumio_retry ALTER COLUMN id SET DEFAULT nextval('public.cm_jumio_retry_id_seq'::regclass);


--
-- TOC entry 3989 (class 2604 OID 20550)
-- Name: cm_notification_history notify_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.cm_notification_history ALTER COLUMN notify_id SET DEFAULT nextval('public.cm_notification_history_notify_id_seq'::regclass);


--
-- TOC entry 3991 (class 2604 OID 20551)
-- Name: cm_rating id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.cm_rating ALTER COLUMN id SET DEFAULT nextval('public.cm_rating_id_seq'::regclass);


--
-- TOC entry 3993 (class 2604 OID 20552)
-- Name: cm_reloan_notification_details id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.cm_reloan_notification_details ALTER COLUMN id SET DEFAULT nextval('public.cm_reloan_notification_details_id_seq'::regclass);


--
-- TOC entry 3995 (class 2604 OID 20553)
-- Name: cm_tongdun_data id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.cm_tongdun_data ALTER COLUMN id SET DEFAULT nextval('public.cm_tongdun_data_id_seq'::regclass);


--
-- TOC entry 3997 (class 2604 OID 20554)
-- Name: cm_tongdun_lazada id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.cm_tongdun_lazada ALTER COLUMN id SET DEFAULT nextval('public.cm_tongdun_lazada_id_seq'::regclass);


--
-- TOC entry 3999 (class 2604 OID 20555)
-- Name: cm_tongdun_shopee id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.cm_tongdun_shopee ALTER COLUMN id SET DEFAULT nextval('public.cm_tongdun_shopee_id_seq'::regclass);


--
-- TOC entry 4001 (class 2604 OID 20556)
-- Name: cm_tongdun_summary id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.cm_tongdun_summary ALTER COLUMN id SET DEFAULT nextval('public.cm_tongdun_summary_id_seq'::regclass);


--
-- TOC entry 4002 (class 2604 OID 20557)
-- Name: cm_zoloz_mapping id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.cm_zoloz_mapping ALTER COLUMN id SET DEFAULT nextval('public.cm_zoloz_mapping_id_seq'::regclass);


--
-- TOC entry 4003 (class 2604 OID 20635)
-- Name: jumiodata id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.jumiodata ALTER COLUMN id SET DEFAULT nextval('public.jumiodata_id_seq'::regclass);


--
-- TOC entry 4005 (class 2604 OID 20636)
-- Name: tbl_application_contract_details contract_no; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tbl_application_contract_details ALTER COLUMN contract_no SET DEFAULT nextval('public.tbl_application_contract_details_contract_no_seq'::regclass);


--
-- TOC entry 4006 (class 2604 OID 20637)
-- Name: tbl_application_defaulter_mapping id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tbl_application_defaulter_mapping ALTER COLUMN id SET DEFAULT nextval('public.tbl_application_defaulter_mapping_id_seq'::regclass);


--
-- TOC entry 4010 (class 2604 OID 20638)
-- Name: tbl_bank_master bank_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tbl_bank_master ALTER COLUMN bank_id SET DEFAULT nextval('public.tbl_bank_master_bank_id_seq'::regclass);


--
-- TOC entry 4014 (class 2604 OID 20639)
-- Name: tbl_deferment_pay deferment_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tbl_deferment_pay ALTER COLUMN deferment_id SET DEFAULT nextval('public.tbl_deferment_pay_deferment_id_seq'::regclass);


--
-- TOC entry 4022 (class 2604 OID 20640)
-- Name: tbl_email_template id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tbl_email_template ALTER COLUMN id SET DEFAULT nextval('public.tbl_email_template_id_seq'::regclass);


--
-- TOC entry 4026 (class 2604 OID 20641)
-- Name: tbl_lod id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tbl_lod ALTER COLUMN id SET DEFAULT nextval('public.tbl_lod_id_seq'::regclass);


--
-- TOC entry 4028 (class 2604 OID 20642)
-- Name: tbl_new_reference_approval id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tbl_new_reference_approval ALTER COLUMN id SET DEFAULT nextval('public.tbl_new_reference_approval_id_seq'::regclass);


--
-- TOC entry 4029 (class 2604 OID 20643)
-- Name: tbl_occupation id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tbl_occupation ALTER COLUMN id SET DEFAULT nextval('public.tbl_occupation_id_seq'::regclass);


--
-- TOC entry 4033 (class 2604 OID 20644)
-- Name: tbl_olddata id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tbl_olddata ALTER COLUMN id SET DEFAULT nextval('public.tbl_olddata_id_seq'::regclass);


--
-- TOC entry 4085 (class 2604 OID 20645)
-- Name: tbl_payment_channel id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tbl_payment_channel ALTER COLUMN id SET DEFAULT nextval('public.tbl_payment_channel_id_seq'::regclass);


--
-- TOC entry 4089 (class 2604 OID 20646)
-- Name: tbl_proof_of_payment id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tbl_proof_of_payment ALTER COLUMN id SET DEFAULT nextval('public.tbl_proof_of_payment_id_seq'::regclass);


--
-- TOC entry 4093 (class 2604 OID 20647)
-- Name: tbl_reason_of_nonpayment reason_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tbl_reason_of_nonpayment ALTER COLUMN reason_id SET DEFAULT nextval('public.tbl_reason_of_nonpayment_reason_id_seq'::regclass);


--
-- TOC entry 4098 (class 2604 OID 20648)
-- Name: tbl_remarks id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tbl_remarks ALTER COLUMN id SET DEFAULT nextval('public.tbl_remarks_id_seq'::regclass);


--
-- TOC entry 4099 (class 2604 OID 20649)
-- Name: tbl_sms_template id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tbl_sms_template ALTER COLUMN id SET DEFAULT nextval('public.tbl_sms_template_id_seq'::regclass);


--
-- TOC entry 4103 (class 2604 OID 20650)
-- Name: tbl_sssno id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tbl_sssno ALTER COLUMN id SET DEFAULT nextval('public.tbl_sssno_id_seq'::regclass);


--
-- TOC entry 4107 (class 2604 OID 20651)
-- Name: tbl_team_agent_mapping id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tbl_team_agent_mapping ALTER COLUMN id SET DEFAULT nextval('public.tbl_team_agent_mapping_id_seq'::regclass);


--
-- TOC entry 4111 (class 2604 OID 20652)
-- Name: tbl_template_template id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tbl_template_template ALTER COLUMN id SET DEFAULT nextval('public.tbl_template_template_id_seq'::regclass);


--
-- TOC entry 4115 (class 2604 OID 20653)
-- Name: tbladdresshistory id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tbladdresshistory ALTER COLUMN id SET DEFAULT nextval('public.tbladdresshistory_id_seq'::regclass);


--
-- TOC entry 4117 (class 2604 OID 20654)
-- Name: tblapplication_emi emi_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tblapplication_emi ALTER COLUMN emi_id SET DEFAULT nextval('public.tblapplication_emi_emi_id_seq'::regclass);


--
-- TOC entry 4124 (class 2604 OID 20655)
-- Name: tblapplication_emi_details emi_detail_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tblapplication_emi_details ALTER COLUMN emi_detail_id SET DEFAULT nextval('public.tblapplication_emi_details_emi_detail_id_seq'::regclass);


--
-- TOC entry 4131 (class 2604 OID 20656)
-- Name: tblapplication_emi_payment payment_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tblapplication_emi_payment ALTER COLUMN payment_id SET DEFAULT nextval('public.tblapplication_emi_payment_payment_id_seq'::regclass);


--
-- TOC entry 4140 (class 2604 OID 20657)
-- Name: tblapplication_payment_proof id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tblapplication_payment_proof ALTER COLUMN id SET DEFAULT nextval('public.tblapplication_payment_proof_id_seq'::regclass);


--
-- TOC entry 4144 (class 2604 OID 20658)
-- Name: tblapplication_personal_verification_details detailid; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tblapplication_personal_verification_details ALTER COLUMN detailid SET DEFAULT nextval('public.tblapplication_personal_verification_details_detailid_seq'::regclass);


--
-- TOC entry 4150 (class 2604 OID 20659)
-- Name: tblapplication_ptp_details ptp_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tblapplication_ptp_details ALTER COLUMN ptp_id SET DEFAULT nextval('public.tblapplication_ptp_details_ptp_id_seq'::regclass);


--
-- TOC entry 4152 (class 2604 OID 20660)
-- Name: tblapplication_record applicationno; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tblapplication_record ALTER COLUMN applicationno SET DEFAULT nextval('public.tblapplication_record_applicationno_seq'::regclass);


--
-- TOC entry 4199 (class 2604 OID 20661)
-- Name: tblapplication_reloan_preterm_details id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tblapplication_reloan_preterm_details ALTER COLUMN id SET DEFAULT nextval('public.tblapplication_reloan_preterm_details_id_seq'::regclass);


--
-- TOC entry 4201 (class 2604 OID 20662)
-- Name: tblapplication_user id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tblapplication_user ALTER COLUMN id SET DEFAULT nextval('public.tblapplication_user_id_seq'::regclass);


--
-- TOC entry 4205 (class 2604 OID 20663)
-- Name: tblapplicationdata_verification_details verification_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tblapplicationdata_verification_details ALTER COLUMN verification_id SET DEFAULT nextval('public.tblapplicationdata_verification_details_verification_id_seq'::regclass);


--
-- TOC entry 4206 (class 2604 OID 20664)
-- Name: tblbarangay id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tblbarangay ALTER COLUMN id SET DEFAULT nextval('public.tblbarangay_id_seq'::regclass);


--
-- TOC entry 4211 (class 2604 OID 20665)
-- Name: tblcity cityid; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tblcity ALTER COLUMN cityid SET DEFAULT nextval('public.tblcity_cityid_seq'::regclass);


--
-- TOC entry 4214 (class 2604 OID 20666)
-- Name: tblcivil_status civilid; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tblcivil_status ALTER COLUMN civilid SET DEFAULT nextval('public.tblcivil_status_civilid_seq'::regclass);


--
-- TOC entry 4215 (class 2604 OID 20667)
-- Name: tbldefaulter_collector_movement_history id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tbldefaulter_collector_movement_history ALTER COLUMN id SET DEFAULT nextval('public.tbldefaulter_collector_movement_history_id_seq'::regclass);


--
-- TOC entry 4217 (class 2604 OID 20668)
-- Name: tbldisbursement_record id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tbldisbursement_record ALTER COLUMN id SET DEFAULT nextval('public.tbldisbursement_record_id_seq'::regclass);


--
-- TOC entry 4220 (class 2604 OID 20669)
-- Name: tblemi_collection_agent_mapping id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tblemi_collection_agent_mapping ALTER COLUMN id SET DEFAULT nextval('public.tblemi_collection_agent_mapping_id_seq'::regclass);


--
-- TOC entry 4223 (class 2604 OID 20670)
-- Name: tblindustry id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tblindustry ALTER COLUMN id SET DEFAULT nextval('public.tblindustry_id_seq'::regclass);


--
-- TOC entry 4225 (class 2604 OID 20671)
-- Name: tblotp id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tblotp ALTER COLUMN id SET DEFAULT nextval('public.tblotp_id_seq'::regclass);


--
-- TOC entry 4228 (class 2604 OID 20672)
-- Name: tblprovince province_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tblprovince ALTER COLUMN province_id SET DEFAULT nextval('public.tblprovince_province_id_seq'::regclass);


--
-- TOC entry 4233 (class 2604 OID 20673)
-- Name: tblterm_type id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tblterm_type ALTER COLUMN id SET DEFAULT nextval('public.tblterm_type_id_seq'::regclass);


--
-- TOC entry 4234 (class 2604 OID 20674)
-- Name: tblterms id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tblterms ALTER COLUMN id SET DEFAULT nextval('public.tblterms_id_seq'::regclass);

ALTER TABLE ONLY public.cm_about_us
    ADD CONSTRAINT cm_about_us_pkey PRIMARY KEY (id);


--
-- TOC entry 4245 (class 2606 OID 20849)
-- Name: cm_contact_us cm_contact_us_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.cm_contact_us
    ADD CONSTRAINT cm_contact_us_pkey PRIMARY KEY (id);


--
-- TOC entry 4247 (class 2606 OID 20851)
-- Name: cm_customer_call_log_details cm_customer_call_log_details_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.cm_customer_call_log_details
    ADD CONSTRAINT cm_customer_call_log_details_pkey PRIMARY KEY (detailid);


--
-- TOC entry 4249 (class 2606 OID 20853)
-- Name: cm_customer_call_log_summary cm_customer_call_log_summary_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.cm_customer_call_log_summary
    ADD CONSTRAINT cm_customer_call_log_summary_pkey PRIMARY KEY (summaryid);


--
-- TOC entry 4251 (class 2606 OID 20855)
-- Name: cm_customer_contact_details cm_customer_contact_details_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.cm_customer_contact_details
    ADD CONSTRAINT cm_customer_contact_details_pkey PRIMARY KEY (contactid);


--
-- TOC entry 4253 (class 2606 OID 20857)
-- Name: cm_customer_fb_details cm_customer_fb_details_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.cm_customer_fb_details
    ADD CONSTRAINT cm_customer_fb_details_pkey PRIMARY KEY (id);


--
-- TOC entry 4255 (class 2606 OID 20859)
-- Name: cm_customer_imei cm_customer_imei_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.cm_customer_imei
    ADD CONSTRAINT cm_customer_imei_pkey PRIMARY KEY (id);


--
-- TOC entry 4257 (class 2606 OID 20861)
-- Name: cm_customer_loan_calc cm_customer_loan_calc_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.cm_customer_loan_calc
    ADD CONSTRAINT cm_customer_loan_calc_pkey PRIMARY KEY (id);


--
-- TOC entry 4259 (class 2606 OID 20863)
-- Name: cm_customer_location cm_customer_location_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.cm_customer_location
    ADD CONSTRAINT cm_customer_location_pkey PRIMARY KEY (locationid);


--
-- TOC entry 4261 (class 2606 OID 20865)
-- Name: cm_customer_notification cm_customer_notification_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.cm_customer_notification
    ADD CONSTRAINT cm_customer_notification_pkey PRIMARY KEY (id);


--
-- TOC entry 4263 (class 2606 OID 20867)
-- Name: cm_faq cm_faq_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.cm_faq
    ADD CONSTRAINT cm_faq_pkey PRIMARY KEY (id);


--
-- TOC entry 4265 (class 2606 OID 20869)
-- Name: cm_jumio_retry cm_jumio_retry_jsondata_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.cm_jumio_retry
    ADD CONSTRAINT cm_jumio_retry_jsondata_key UNIQUE (jsondata);


--
-- TOC entry 4267 (class 2606 OID 20871)
-- Name: cm_jumio_retry cm_jumio_retry_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.cm_jumio_retry
    ADD CONSTRAINT cm_jumio_retry_pkey PRIMARY KEY (id);


--
-- TOC entry 4269 (class 2606 OID 20873)
-- Name: cm_notification_history cm_notification_history_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.cm_notification_history
    ADD CONSTRAINT cm_notification_history_pkey PRIMARY KEY (notify_id);


--
-- TOC entry 4271 (class 2606 OID 20875)
-- Name: cm_privacy_policy cm_privacy_policy_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.cm_privacy_policy
    ADD CONSTRAINT cm_privacy_policy_pkey PRIMARY KEY (id);


--
-- TOC entry 4273 (class 2606 OID 20877)
-- Name: cm_rating cm_rating_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.cm_rating
    ADD CONSTRAINT cm_rating_pkey PRIMARY KEY (id);


--
-- TOC entry 4275 (class 2606 OID 20879)
-- Name: cm_reloan_notification_details cm_reloan_notification_details_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.cm_reloan_notification_details
    ADD CONSTRAINT cm_reloan_notification_details_pkey PRIMARY KEY (id);


--
-- TOC entry 4277 (class 2606 OID 20881)
-- Name: cm_tongdun_data cm_tongdun_data_order_no_userid_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.cm_tongdun_data
    ADD CONSTRAINT cm_tongdun_data_order_no_userid_key UNIQUE (order_no, userid);


--
-- TOC entry 4279 (class 2606 OID 20883)
-- Name: cm_tongdun_data cm_tongdun_data_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.cm_tongdun_data
    ADD CONSTRAINT cm_tongdun_data_pkey PRIMARY KEY (id);


--
-- TOC entry 4282 (class 2606 OID 20885)
-- Name: cm_tongdun_lazada cm_tongdun_lazada_order_no_jumio_reference_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.cm_tongdun_lazada
    ADD CONSTRAINT cm_tongdun_lazada_order_no_jumio_reference_key UNIQUE (order_no, jumio_reference);


--
-- TOC entry 4284 (class 2606 OID 20887)
-- Name: cm_tongdun_lazada cm_tongdun_lazada_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.cm_tongdun_lazada
    ADD CONSTRAINT cm_tongdun_lazada_pkey PRIMARY KEY (id);


--
-- TOC entry 4286 (class 2606 OID 20889)
-- Name: cm_tongdun_shopee cm_tongdun_shopee_order_no_jumio_reference_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.cm_tongdun_shopee
    ADD CONSTRAINT cm_tongdun_shopee_order_no_jumio_reference_key UNIQUE (order_no, jumio_reference);


--
-- TOC entry 4288 (class 2606 OID 20891)
-- Name: cm_tongdun_shopee cm_tongdun_shopee_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.cm_tongdun_shopee
    ADD CONSTRAINT cm_tongdun_shopee_pkey PRIMARY KEY (id);


--
-- TOC entry 4290 (class 2606 OID 20893)
-- Name: cm_tongdun_summary cm_tongdun_summary_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.cm_tongdun_summary
    ADD CONSTRAINT cm_tongdun_summary_pkey PRIMARY KEY (id);


--
-- TOC entry 4292 (class 2606 OID 21100)
-- Name: jumiodata jumiodata_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.jumiodata
    ADD CONSTRAINT jumiodata_pkey PRIMARY KEY (id);


--
-- TOC entry 4356 (class 2606 OID 21102)
-- Name: tblapplication_record pk_applicationno; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tblapplication_record
    ADD CONSTRAINT pk_applicationno PRIMARY KEY (applicationno);


--
-- TOC entry 4384 (class 2606 OID 21106)
-- Name: tblcity pk_cityid; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tblcity
    ADD CONSTRAINT pk_cityid PRIMARY KEY (cityid);


--
-- TOC entry 4389 (class 2606 OID 21108)
-- Name: tblcivil_status pk_civilid; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tblcivil_status
    ADD CONSTRAINT pk_civilid PRIMARY KEY (civilid);


--
-- TOC entry 4403 (class 2606 OID 21110)
-- Name: tblterm_type pk_termtypeid; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tblterm_type
    ADD CONSTRAINT pk_termtypeid PRIMARY KEY (id);


--
-- TOC entry 4349 (class 2606 OID 21112)
-- Name: tblapplication_personal_verification_details pk_varification_details; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tblapplication_personal_verification_details
    ADD CONSTRAINT pk_varification_details PRIMARY KEY (detailid);


--
-- TOC entry 4295 (class 2606 OID 21114)
-- Name: tbl_application_contract_details tbl_application_contract_details_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tbl_application_contract_details
    ADD CONSTRAINT tbl_application_contract_details_pkey PRIMARY KEY (contract_no);


--
-- TOC entry 4297 (class 2606 OID 21116)
-- Name: tbl_application_defaulter_mapping tbl_application_defaulter_mapping_application_no_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tbl_application_defaulter_mapping
    ADD CONSTRAINT tbl_application_defaulter_mapping_application_no_key UNIQUE (application_no);


--
-- TOC entry 4299 (class 2606 OID 21118)
-- Name: tbl_application_defaulter_mapping tbl_application_defaulter_mapping_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tbl_application_defaulter_mapping
    ADD CONSTRAINT tbl_application_defaulter_mapping_pkey PRIMARY KEY (id);


--
-- TOC entry 4354 (class 2606 OID 21120)
-- Name: tblapplication_ptp_details tbl_application_ptp_details_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tblapplication_ptp_details
    ADD CONSTRAINT tbl_application_ptp_details_pkey PRIMARY KEY (ptp_id);


--
-- TOC entry 4301 (class 2606 OID 21122)
-- Name: tbl_bank_master tbl_bank_master_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tbl_bank_master
    ADD CONSTRAINT tbl_bank_master_pkey PRIMARY KEY (bank_id);


--
-- TOC entry 4303 (class 2606 OID 21124)
-- Name: tbl_deferment_pay tbl_deferment_pay_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tbl_deferment_pay
    ADD CONSTRAINT tbl_deferment_pay_pkey PRIMARY KEY (deferment_id);


--
-- TOC entry 4305 (class 2606 OID 21126)
-- Name: tbl_email_template tbl_email_template_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tbl_email_template
    ADD CONSTRAINT tbl_email_template_pkey PRIMARY KEY (id);


--
-- TOC entry 4307 (class 2606 OID 21128)
-- Name: tbl_lod tbl_lod_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tbl_lod
    ADD CONSTRAINT tbl_lod_pkey PRIMARY KEY (id);


--
-- TOC entry 4309 (class 2606 OID 21130)
-- Name: tbl_new_reference_approval tbl_new_reference_approval_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tbl_new_reference_approval
    ADD CONSTRAINT tbl_new_reference_approval_pkey PRIMARY KEY (id);


--
-- TOC entry 4311 (class 2606 OID 21132)
-- Name: tbl_occupation tbl_occupation_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tbl_occupation
    ADD CONSTRAINT tbl_occupation_pkey PRIMARY KEY (id);


--
-- TOC entry 4313 (class 2606 OID 21134)
-- Name: tbl_olddata tbl_olddata_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tbl_olddata
    ADD CONSTRAINT tbl_olddata_pkey PRIMARY KEY (id);


--
-- TOC entry 4315 (class 2606 OID 21136)
-- Name: tbl_payment_channel tbl_payment_channel_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tbl_payment_channel
    ADD CONSTRAINT tbl_payment_channel_pkey PRIMARY KEY (id);


--
-- TOC entry 4317 (class 2606 OID 21138)
-- Name: tbl_proof_of_payment tbl_proof_of_payment_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tbl_proof_of_payment
    ADD CONSTRAINT tbl_proof_of_payment_pkey PRIMARY KEY (id);


--
-- TOC entry 4319 (class 2606 OID 21140)
-- Name: tbl_reason_of_nonpayment tbl_reason_of_nonpayment_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tbl_reason_of_nonpayment
    ADD CONSTRAINT tbl_reason_of_nonpayment_pkey PRIMARY KEY (reason_id);


--
-- TOC entry 4321 (class 2606 OID 21142)
-- Name: tbl_remarks tbl_remarks_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tbl_remarks
    ADD CONSTRAINT tbl_remarks_pkey PRIMARY KEY (id);


--
-- TOC entry 4323 (class 2606 OID 21144)
-- Name: tbl_sms_template tbl_sms_template_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tbl_sms_template
    ADD CONSTRAINT tbl_sms_template_pkey PRIMARY KEY (id);


--
-- TOC entry 4325 (class 2606 OID 21146)
-- Name: tbl_sssno tbl_sssno_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tbl_sssno
    ADD CONSTRAINT tbl_sssno_pkey PRIMARY KEY (id);


--
-- TOC entry 4327 (class 2606 OID 21148)
-- Name: tbl_team_agent_mapping tbl_team_agent_mapping_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tbl_team_agent_mapping
    ADD CONSTRAINT tbl_team_agent_mapping_pkey PRIMARY KEY (id);


--
-- TOC entry 4329 (class 2606 OID 21150)
-- Name: tbl_template_template tbl_template_template_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tbl_template_template
    ADD CONSTRAINT tbl_template_template_pkey PRIMARY KEY (id);


--
-- TOC entry 4331 (class 2606 OID 21152)
-- Name: tbladdresshistory tbladdresshistory_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tbladdresshistory
    ADD CONSTRAINT tbladdresshistory_pkey PRIMARY KEY (id);


--
-- TOC entry 4334 (class 2606 OID 21154)
-- Name: tblapplication_emi tblapplication_emi_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tblapplication_emi
    ADD CONSTRAINT tblapplication_emi_pkey PRIMARY KEY (emi_id);


--
-- TOC entry 4346 (class 2606 OID 21156)
-- Name: tblapplication_payment_proof tblapplication_payment_proof_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tblapplication_payment_proof
    ADD CONSTRAINT tblapplication_payment_proof_pkey PRIMARY KEY (id);


--
-- TOC entry 4370 (class 2606 OID 21158)
-- Name: tblapplication_reloan_preterm_details tblapplication_reloan_preterm_details_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tblapplication_reloan_preterm_details
    ADD CONSTRAINT tblapplication_reloan_preterm_details_pkey PRIMARY KEY (id);


--
-- TOC entry 4372 (class 2606 OID 21160)
-- Name: tblapplication_user tblapplication_user_personalemail_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tblapplication_user
    ADD CONSTRAINT tblapplication_user_personalemail_key UNIQUE (personalemail);


--
-- TOC entry 4374 (class 2606 OID 21162)
-- Name: tblapplication_user tblapplication_user_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tblapplication_user
    ADD CONSTRAINT tblapplication_user_pkey PRIMARY KEY (id);


--
-- TOC entry 4376 (class 2606 OID 21164)
-- Name: tblapplicationdata_verification_details tblapplicationdata_verification_details_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tblapplicationdata_verification_details
    ADD CONSTRAINT tblapplicationdata_verification_details_pkey PRIMARY KEY (verification_id);


--
-- TOC entry 4339 (class 2606 OID 21166)
-- Name: tblapplication_emi_details tblassociate_emi_details_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tblapplication_emi_details
    ADD CONSTRAINT tblassociate_emi_details_pkey PRIMARY KEY (emi_detail_id);


--
-- TOC entry 4344 (class 2606 OID 21168)
-- Name: tblapplication_emi_payment tblassociate_emi_payment_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tblapplication_emi_payment
    ADD CONSTRAINT tblassociate_emi_payment_pkey PRIMARY KEY (payment_id);


--
-- TOC entry 4382 (class 2606 OID 21170)
-- Name: tblbarangay tblbarangay_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tblbarangay
    ADD CONSTRAINT tblbarangay_pkey PRIMARY KEY (id);


--
-- TOC entry 4391 (class 2606 OID 21172)
-- Name: tbldefaulter_collector_movement_history tbldefaulter_collector_movement_history_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tbldefaulter_collector_movement_history
    ADD CONSTRAINT tbldefaulter_collector_movement_history_pkey PRIMARY KEY (id);


--
-- TOC entry 4393 (class 2606 OID 21174)
-- Name: tbldisbursement_record tbldisbursement_record_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tbldisbursement_record
    ADD CONSTRAINT tbldisbursement_record_pkey PRIMARY KEY (id);


--
-- TOC entry 4395 (class 2606 OID 21176)
-- Name: tblemi_collection_agent_mapping tblemi_collection_agent_mapping_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tblemi_collection_agent_mapping
    ADD CONSTRAINT tblemi_collection_agent_mapping_pkey PRIMARY KEY (id);


--
-- TOC entry 4397 (class 2606 OID 21178)
-- Name: tblindustry tblindustry_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tblindustry
    ADD CONSTRAINT tblindustry_pkey PRIMARY KEY (id);


--
-- TOC entry 4399 (class 2606 OID 21180)
-- Name: tblprovince tblprovince_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tblprovince
    ADD CONSTRAINT tblprovince_pkey PRIMARY KEY (province_id);


--
-- TOC entry 4405 (class 2606 OID 21182)
-- Name: tblterms tblterms_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tblterms
    ADD CONSTRAINT tblterms_pkey PRIMARY KEY (id);


--
-- TOC entry 4280 (class 1259 OID 21193)
-- Name: cm_tongdun_data_userid_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX cm_tongdun_data_userid_idx ON public.cm_tongdun_data USING btree (userid);


--
-- TOC entry 4293 (class 1259 OID 21194)
-- Name: contract_no_index; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX contract_no_index ON public.tbl_application_contract_details USING btree (contract_no);


--
-- TOC entry 4347 (class 1259 OID 21518)
-- Name: detailid_index; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX detailid_index ON public.tblapplication_personal_verification_details USING btree (detailid);


--
-- TOC entry 4335 (class 1259 OID 21519)
-- Name: emi_detail_id_index; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX emi_detail_id_index ON public.tblapplication_emi_details USING btree (emi_detail_id);


--
-- TOC entry 4336 (class 1259 OID 21520)
-- Name: emi_details_balance_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX emi_details_balance_idx ON public.tblapplication_emi_details USING btree (balanceamount);


--
-- TOC entry 4337 (class 1259 OID 21521)
-- Name: emi_details_emi_date_index; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX emi_details_emi_date_index ON public.tblapplication_emi_details USING btree (emi_date);


--
-- TOC entry 4332 (class 1259 OID 21522)
-- Name: emi_id_index; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX emi_id_index ON public.tblapplication_emi USING btree (emi_id);


--
-- TOC entry 4340 (class 1259 OID 21523)
-- Name: payment_id_index; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX payment_id_index ON public.tblapplication_emi_payment USING btree (payment_id);


--
-- TOC entry 4341 (class 1259 OID 21524)
-- Name: payment_paid_amount_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX payment_paid_amount_idx ON public.tblapplication_emi_payment USING btree (paid_amount);


--
-- TOC entry 4342 (class 1259 OID 21525)
-- Name: payment_paid_on_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX payment_paid_on_idx ON public.tblapplication_emi_payment USING btree (paid_on);


--
-- TOC entry 4350 (class 1259 OID 21526)
-- Name: pto_current_ptp_on_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX pto_current_ptp_on_idx ON public.tblapplication_ptp_details USING btree (current_date_ptp);


--
-- TOC entry 4351 (class 1259 OID 21527)
-- Name: ptp_application_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX ptp_application_idx ON public.tblapplication_ptp_details USING btree (application_on);


--
-- TOC entry 4352 (class 1259 OID 21528)
-- Name: ptp_id_index; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX ptp_id_index ON public.tblapplication_ptp_details USING btree (ptp_id);


--
-- TOC entry 4357 (class 1259 OID 21529)
-- Name: tblapplication_applicationno_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX tblapplication_applicationno_idx ON public.tblapplication_record USING btree (applicationno);


--
-- TOC entry 4358 (class 1259 OID 21530)
-- Name: tblapplication_record_createdon_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX tblapplication_record_createdon_idx ON public.tblapplication_record USING btree (createdon);


--
-- TOC entry 4359 (class 1259 OID 21531)
-- Name: tblapplication_record_index; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX tblapplication_record_index ON public.tblapplication_record USING btree (applicationno);


--
-- TOC entry 4360 (class 1259 OID 21532)
-- Name: tblapplication_record_is_preterm_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX tblapplication_record_is_preterm_idx ON public.tblapplication_record USING btree (is_preterm);


--
-- TOC entry 4361 (class 1259 OID 21533)
-- Name: tblapplication_record_isapproved_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX tblapplication_record_isapproved_idx ON public.tblapplication_record USING btree (isapproved);


--
-- TOC entry 4362 (class 1259 OID 21534)
-- Name: tblapplication_record_ischeck_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX tblapplication_record_ischeck_idx ON public.tblapplication_record USING btree (ischeck);


--
-- TOC entry 4363 (class 1259 OID 21535)
-- Name: tblapplication_record_isdisbursedexport_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX tblapplication_record_isdisbursedexport_idx ON public.tblapplication_record USING btree (isdisbursedexport);


--
-- TOC entry 4364 (class 1259 OID 21536)
-- Name: tblapplication_record_isfor_reloan_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX tblapplication_record_isfor_reloan_idx ON public.tblapplication_record USING btree (isfor_reloan);


--
-- TOC entry 4365 (class 1259 OID 21537)
-- Name: tblapplication_record_isfordisbursement_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX tblapplication_record_isfordisbursement_idx ON public.tblapplication_record USING btree (isfordisbursement);


--
-- TOC entry 4366 (class 1259 OID 21538)
-- Name: tblapplication_record_isrecheck_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX tblapplication_record_isrecheck_idx ON public.tblapplication_record USING btree (isrecheck);


--
-- TOC entry 4367 (class 1259 OID 21539)
-- Name: tblapplication_record_isreverified_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX tblapplication_record_isreverified_idx ON public.tblapplication_record USING btree (isreverified);


--
-- TOC entry 4368 (class 1259 OID 21540)
-- Name: tblapplication_record_isverified_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX tblapplication_record_isverified_idx ON public.tblapplication_record USING btree (isverified);


--
-- TOC entry 4378 (class 1259 OID 21541)
-- Name: tblbarangay_barangay_name_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX tblbarangay_barangay_name_idx ON public.tblbarangay USING btree (barangay_name);


--
-- TOC entry 4379 (class 1259 OID 21542)
-- Name: tblbarangay_city_id_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX tblbarangay_city_id_idx ON public.tblbarangay USING btree (city_id);


--
-- TOC entry 4380 (class 1259 OID 21543)
-- Name: tblbarangay_id_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX tblbarangay_id_idx ON public.tblbarangay USING btree (id);


--
-- TOC entry 4385 (class 1259 OID 21544)
-- Name: tblcity_cityid_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX tblcity_cityid_idx ON public.tblcity USING btree (cityid);


--
-- TOC entry 4386 (class 1259 OID 21545)
-- Name: tblcity_cityname_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX tblcity_cityname_idx ON public.tblcity USING btree (cityname);


--
-- TOC entry 4387 (class 1259 OID 21546)
-- Name: tblcity_province_id_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX tblcity_province_id_idx ON public.tblcity USING btree (province_id);


--
-- TOC entry 4400 (class 1259 OID 21547)
-- Name: tblprovince_province_id_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX tblprovince_province_id_idx ON public.tblprovince USING btree (province_id);


--
-- TOC entry 4401 (class 1259 OID 21548)
-- Name: tblprovince_province_name_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX tblprovince_province_name_idx ON public.tblprovince USING btree (province_name);


--
-- TOC entry 4377 (class 1259 OID 21577)
-- Name: verification_id_index; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX verification_id_index ON public.tblapplicationdata_verification_details USING btree (verification_id);


--
-- TOC entry 4406 (class 2606 OID 21980)
-- Name: cm_rating fk_UserId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.cm_rating
    ADD CONSTRAINT "fk_UserId" FOREIGN KEY (userid) REFERENCES public.tblapplication_user(id) NOT VALID;


--
-- TOC entry 4418 (class 2606 OID 21985)
-- Name: tblterms fk_id_termid; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tblterms
    ADD CONSTRAINT fk_id_termid FOREIGN KEY (term_id) REFERENCES public.tblterm_type(id);


--
-- TOC entry 4407 (class 2606 OID 21990)
-- Name: tbl_application_contract_details tbl_application_contract_details_application_no_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tbl_application_contract_details
    ADD CONSTRAINT tbl_application_contract_details_application_no_fkey FOREIGN KEY (application_no) REFERENCES public.tblapplication_record(applicationno) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- TOC entry 4412 (class 2606 OID 21995)
-- Name: tblapplication_ptp_details tbl_application_ptp_details_application_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tblapplication_ptp_details
    ADD CONSTRAINT tbl_application_ptp_details_application_id_fkey FOREIGN KEY (application_on) REFERENCES public.tblapplication_record(applicationno) ON DELETE CASCADE;


--
-- TOC entry 4408 (class 2606 OID 22000)
-- Name: tblapplication_emi tblapplication_emi_application_no_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tblapplication_emi
    ADD CONSTRAINT tblapplication_emi_application_no_fkey FOREIGN KEY (application_no) REFERENCES public.tblapplication_record(applicationno) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- TOC entry 4409 (class 2606 OID 22005)
-- Name: tblapplication_emi_details tblapplication_emi_details_emi_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tblapplication_emi_details
    ADD CONSTRAINT tblapplication_emi_details_emi_id_fkey FOREIGN KEY (emi_id) REFERENCES public.tblapplication_emi(emi_id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- TOC entry 4410 (class 2606 OID 22010)
-- Name: tblapplication_emi_payment tblapplication_emi_payment_application_no_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tblapplication_emi_payment
    ADD CONSTRAINT tblapplication_emi_payment_application_no_fkey FOREIGN KEY (application_no) REFERENCES public.tblapplication_record(applicationno) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- TOC entry 4413 (class 2606 OID 22015)
-- Name: tblapplication_record tblapplication_record_user_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tblapplication_record
    ADD CONSTRAINT tblapplication_record_user_id_fkey FOREIGN KEY (user_id) REFERENCES public.tblapplication_user(id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- TOC entry 4414 (class 2606 OID 22021)
-- Name: tblapplicationdata_verification_details tblapplicationdata_verification_details_application_no_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tblapplicationdata_verification_details
    ADD CONSTRAINT tblapplicationdata_verification_details_application_no_fkey FOREIGN KEY (application_no) REFERENCES public.tblapplication_record(applicationno) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- TOC entry 4411 (class 2606 OID 22026)
-- Name: tblapplication_personal_verification_details tblapplicationvierificationdetails_applicationid_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tblapplication_personal_verification_details
    ADD CONSTRAINT tblapplicationvierificationdetails_applicationid_fkey FOREIGN KEY (application_no) REFERENCES public.tblapplication_record(applicationno) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- TOC entry 4415 (class 2606 OID 22032)
-- Name: tblbarangay tblbarangay_city_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tblbarangay
    ADD CONSTRAINT tblbarangay_city_id_fkey FOREIGN KEY (city_id) REFERENCES public.tblcity(cityid) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- TOC entry 4416 (class 2606 OID 22037)
-- Name: tblcity tblcity_province_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tblcity
    ADD CONSTRAINT tblcity_province_id_fkey FOREIGN KEY (province_id) REFERENCES public.tblprovince(province_id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- TOC entry 4417 (class 2606 OID 22042)
-- Name: tblemi_collection_agent_mapping tblemi_collection_agent_mapping_emi_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tblemi_collection_agent_mapping
    ADD CONSTRAINT tblemi_collection_agent_mapping_emi_id_fkey FOREIGN KEY (emi_id) REFERENCES public.tblapplication_emi(emi_id) ON UPDATE CASCADE ON DELETE CASCADE;
