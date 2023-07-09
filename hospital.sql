-- phpMyAdmin SQL Dump
-- version 5.2.0
-- https://www.phpmyadmin.net/
--
-- Host: 127.0.0.1
-- Generation Time: Jul 05, 2023 at 03:29 PM
-- Server version: 10.4.27-MariaDB
-- PHP Version: 8.2.0

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Database: `hospital`
--

DELIMITER $$
--
-- Functions
--
CREATE DEFINER=`root`@`localhost` FUNCTION `count_checkup_by_doctor` (`doctor_id` INT) RETURNS INT(11)  BEGIN
    DECLARE hitung INT;
    SELECT COUNT(*) INTO hitung FROM checkup WHERE doctor_id = doctor_id;
    RETURN hitung;
END$$

DELIMITER ;

-- --------------------------------------------------------

--
-- Table structure for table `checkup`
--

CREATE TABLE `checkup` (
  `id` bigint(20) NOT NULL,
  `doctor_id` int(11) NOT NULL,
  `patient_id` int(11) NOT NULL,
  `checkup_date` date NOT NULL,
  `diagnose` varchar(500) NOT NULL,
  `treatment` varchar(500) NOT NULL,
  `cost` decimal(12,2) NOT NULL,
  `total_cost` decimal(12,2) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `checkup`
--

INSERT INTO `checkup` (`id`, `doctor_id`, `patient_id`, `checkup_date`, `diagnose`, `treatment`, `cost`, `total_cost`) VALUES
(1, 1, 1, '2023-04-03', 'batuk pilek', '', '200000.00', '2310000.00'),
(2, 2, 3, '2023-04-05', 'patah tulang', 'operasi', '25000000.00', '25000000.00'),
(3, 1, 7, '2023-04-02', 'demam', '-', '100000.00', '150000.00'),
(4, 2, 2, '2023-04-11', 'keseleo', 'fisioterapi', '100000.00', '165000.00'),
(5, 4, 5, '2023-04-10', 'baik-baik saja', '-', '200000.00', '235000.00'),
(6, 3, 1, '2023-03-30', 'baik-baik saja', '-', '500000.00', '500000.00'),
(7, 1, 9, '2023-05-02', 'diare', '', '100000.00', '100000.00'),
(8, 1, 9, '2023-05-02', 'diare', '', '100000.00', '100000.00'),
(9, 5, 9, '2023-05-02', 'diare', '', '100000.00', '130000.00'),
(10, 1, 9, '2023-05-02', 'diare', '', '100000.00', '130000.00');

--
-- Triggers `checkup`
--
DELIMITER $$
CREATE TRIGGER `update_total_cost_on_cost_update` BEFORE UPDATE ON `checkup` FOR EACH ROW BEGIN
    SET NEW.total_cost = NEW.cost + (SELECT SUM(quantity * price)
        FROM checkup_medicine
        WHERE checkup_id = NEW.id);
END
$$
DELIMITER ;

-- --------------------------------------------------------

--
-- Table structure for table `checkup_medicine`
--

CREATE TABLE `checkup_medicine` (
  `checkup_id` bigint(20) NOT NULL,
  `drug_id` int(11) NOT NULL,
  `daily_intake` int(11) DEFAULT NULL,
  `description` varchar(255) DEFAULT NULL,
  `quantity` tinyint(4) DEFAULT NULL,
  `price` decimal(12,2) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `checkup_medicine`
--

INSERT INTO `checkup_medicine` (`checkup_id`, `drug_id`, `daily_intake`, `description`, `quantity`, `price`) VALUES
(1, 3, 3, 'after meals', 1, '25000.00'),
(1, 7, 3, 'after meals', 1, '55000.00'),
(1, 13, 3, 'after meals', 1, '30000.00'),
(1, 20, 2, 'pagi dan sore', 2, '1000000.00'),
(3, 2, 3, 'after meals', 1, '15000.00'),
(3, 20, 1, 'if needed', 1, '35000.00'),
(4, 17, 1, 'if needed', 1, '47500.00'),
(4, 18, 1, 'morning & night', 1, '17500.00'),
(5, 20, 1, 'if needed', 1, '35000.00'),
(9, 15, 3, 'after meals, 2 tablet at once', 2, '15000.00'),
(10, 15, 3, 'after meals, 2 tablet at once', 2, '15000.00');

--
-- Triggers `checkup_medicine`
--
DELIMITER $$
CREATE TRIGGER `increase_drug_quantity` AFTER DELETE ON `checkup_medicine` FOR EACH ROW BEGIN
 UPDATE drugs
 SET quantity = quantity + OLD.quantity
 WHERE drugs.id = OLD.drug_id;
END
$$
DELIMITER ;
DELIMITER $$
CREATE TRIGGER `reduce_drug_quantity` AFTER INSERT ON `checkup_medicine` FOR EACH ROW BEGIN
 UPDATE drugs
 SET quantity = quantity - NEW.quantity
 WHERE drugs.id = NEW.drug_id;
END
$$
DELIMITER ;
DELIMITER $$
CREATE TRIGGER `update_total_cost` AFTER INSERT ON `checkup_medicine` FOR EACH ROW BEGIN
 UPDATE checkup c
 SET total_cost = (SELECT SUM(quantity * price) FROM checkup_medicine WHERE checkup_id = NEW.checkup_id)
 WHERE c.id = NEW.checkup_id;
END
$$
DELIMITER ;
DELIMITER $$
CREATE TRIGGER `update_total_cost_on_delete` AFTER DELETE ON `checkup_medicine` FOR EACH ROW BEGIN
  UPDATE checkup c
  SET total_cost = (SELECT SUM(quantity * price) FROM checkup_medicine WHERE checkup_id = OLD.checkup_id)
  WHERE c.id = OLD.checkup_id;
END
$$
DELIMITER ;

-- --------------------------------------------------------

--
-- Stand-in structure for view `daftar_dokter`
-- (See below for the actual view)
--
CREATE TABLE `daftar_dokter` (
`ID` int(11)
,`Nama Lengkap` varchar(200)
,`NIK` varchar(16)
,`Tanggal Lahir` date
,`Tempat Lahir` varchar(50)
,`Jenis Kelamin` enum('M','F')
,`Spesialisasi` varchar(200)
);

-- --------------------------------------------------------

--
-- Stand-in structure for view `data_obat`
-- (See below for the actual view)
--
CREATE TABLE `data_obat` (
`ID` int(11)
,`Nama Obat` varchar(200)
,`Indikasi` varchar(50)
,`Kuantitas` int(11)
,`Harga` decimal(15,2)
);

-- --------------------------------------------------------

--
-- Stand-in structure for view `data_pasien`
-- (See below for the actual view)
--
CREATE TABLE `data_pasien` (
`ID` int(11)
,`Nama Lengkap` varchar(100)
,`Tanggal Lahir` date
,`Tempat Lahir` varchar(50)
,`Jenis Kelamin` enum('M','F')
,`Pekerjaan` varchar(100)
);

-- --------------------------------------------------------

--
-- Table structure for table `doctors`
--

CREATE TABLE `doctors` (
  `id` int(11) NOT NULL,
  `fullname` varchar(200) NOT NULL,
  `nik` varchar(16) NOT NULL,
  `birth_date` date NOT NULL,
  `birth_place` varchar(50) NOT NULL,
  `sex` enum('M','F') NOT NULL,
  `type_id` smallint(5) UNSIGNED DEFAULT NULL,
  `email` varchar(200) DEFAULT NULL,
  `alumni` varchar(200) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `doctors`
--

INSERT INTO `doctors` (`id`, `fullname`, `nik`, `birth_date`, `birth_place`, `sex`, `type_id`, `email`, `alumni`) VALUES
(1, 'dr. Lola Gardner', '', '1993-06-15', 'Boston', 'F', 1, NULL, NULL),
(2, 'dr. Billy Wheeler, Sp.OT', '', '1988-05-11', '', 'M', 2, NULL, NULL),
(3, 'dr. Joel Brady, Sp.KJ', '', '1990-01-09', '', 'M', 9, NULL, NULL),
(4, 'dr. Sean Stewart, Sp.JP', '', '1991-09-23', '', 'M', 4, NULL, NULL),
(5, 'dr. Bartholomew Fenton', '', '1995-08-17', '', 'M', 1, NULL, NULL),
(6, 'dr. Dillon Kip', '', '1990-09-01', '', 'M', 1, NULL, NULL);

-- --------------------------------------------------------

--
-- Table structure for table `doctor_schedule`
--

CREATE TABLE `doctor_schedule` (
  `id` int(11) NOT NULL,
  `doctor_id` int(11) NOT NULL,
  `description` varchar(255) NOT NULL,
  `day` varchar(32) NOT NULL,
  `time` varchar(32) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Table structure for table `doctor_type`
--

CREATE TABLE `doctor_type` (
  `id` smallint(5) UNSIGNED NOT NULL,
  `type` varchar(200) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `doctor_type`
--

INSERT INTO `doctor_type` (`id`, `type`) VALUES
(1, 'General Practitioner'),
(2, 'Orthopedics'),
(3, 'Obgyn'),
(4, 'Neurology'),
(5, 'Cardiology and Vascular Medicine'),
(6, 'Pediatric'),
(7, 'Anestesiology'),
(8, 'Pulmonology and Respiratory Medicine	'),
(9, 'Psychiatry');

-- --------------------------------------------------------

--
-- Table structure for table `drugs`
--

CREATE TABLE `drugs` (
  `id` int(11) NOT NULL,
  `name` varchar(200) NOT NULL,
  `diagnose` varchar(50) NOT NULL,
  `quantity` int(11) NOT NULL,
  `price` decimal(15,2) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `drugs`
--

INSERT INTO `drugs` (`id`, `name`, `diagnose`, `quantity`, `price`) VALUES
(1, 'Rhinos Junior Sirup 60 ml', 'pilek anak', 100, '50000.00'),
(2, 'Paratusin 10 Tablet', 'demam', 75, '15000.00'),
(3, 'Tremenza Sirup 60 ml', 'pilek', 100, '25000.00'),
(4, 'Demacolin 10 Tablet', 'flu', 50, '7500.00'),
(5, 'Actifed Plus Cough Supressant Sirup 60 ml (Merah)', 'batuk kering', 50, '60000.00'),
(6, 'Rhinofed Sirup 60 ml', 'pilek', 50, '37500.00'),
(7, 'Actifed Plus Expectorant Sirup 60 ml (Hijau)', 'batuk berdahak', 50, '55000.00'),
(8, 'Sterimar Nose Hygiene Baby 50 ml', 'tht bayi', 10, '175000.00'),
(9, 'Degirol Hisap 0.25 mg 10 Tablet', 'radang', 100, '10000.00'),
(10, 'Promag Suspensi 60 ml', 'maag', 100, '12500.00'),
(11, 'New Diatabs 4 Tablet', 'diare', 200, '2700.00'),
(12, 'Ketoconazole 200 mg 10 Tablet', 'anti jamur', 50, '8000.00'),
(13, 'Amoxsan Dry Sirup 60 ml', 'antibiotik', 100, '30000.00'),
(14, 'Promag 10 Tablet', 'maag', 100, '8000.00'),
(15, 'Entrostop 20 Tablet', 'diare', 100, '15000.00'),
(16, 'Zyloric 100 mg 10 Tablet', 'asam urat', 100, '27500.00'),
(17, 'Counterpain Cool Gel 30 g', 'nyeri otot', 100, '47500.00'),
(18, 'Estalex 50 mg 10 Tablet', 'pelemas otot', 100, '17500.00'),
(19, 'Ossoral 800 mg 10 Kaplet', 'osteoporosis', 100, '50000.00'),
(20, 'Prove D3-1000 IU 10 Tablet', 'vitamin', 98, '35000.00'),
(21, 'Cindala 10 mg/g Gel 10 g', 'jerawat', 75, '28000.00'),
(22, 'Ciprofloxacin 500 mg 10 Tablet', 'antibiotik', 500, '7500.00');

-- --------------------------------------------------------

--
-- Table structure for table `patients`
--

CREATE TABLE `patients` (
  `id` int(11) NOT NULL,
  `fullname` varchar(100) DEFAULT NULL,
  `birth_date` date DEFAULT NULL,
  `birth_place` varchar(50) DEFAULT NULL,
  `sex` enum('M','F') DEFAULT NULL,
  `occupation` varchar(100) DEFAULT NULL,
  `entry_date` datetime DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `patients`
--

INSERT INTO `patients` (`id`, `fullname`, `birth_date`, `birth_place`, `sex`, `occupation`, `entry_date`) VALUES
(1, 'Reza Rahadian', '1987-03-05', 'Bogor', 'M', 'Actor', '2023-03-02 00:00:00'),
(2, 'Fedi Nuril', '1982-07-01', 'Jakarta', 'M', 'Actor', NULL),
(3, 'Nicholas Saputra Schubring', '1984-02-24', 'Jakarta', 'M', 'Actor', '2023-03-02 00:00:00'),
(4, 'Diandra Paramitha Sastrowardoyo', '1982-03-16', 'Jakarta', 'F', 'Actress', '2023-03-02 00:00:00'),
(5, 'Isyana Sarasvati', '1993-05-02', 'Bandung', 'F', 'Singer', '2023-03-02 13:53:40'),
(6, 'Yoshua Sudarso', '1989-04-12', 'Jakarta', 'M', 'Actor', '2023-03-02 13:53:40'),
(7, 'Arafah Rianti', '1997-09-02', 'Depok', 'F', 'Comedian', '2023-03-02 13:53:40'),
(8, 'G.M.A. Bintang Mahaputra', '1996-05-05', 'Jakarta', 'M', 'Comedian', '2023-03-02 13:53:40'),
(9, 'Aci Resti', '1997-08-12', 'Tangerang', 'F', 'Comedian', '2023-03-02 13:53:40'),
(10, 'Sissy Priscillia', '1985-04-05', 'Jakarta', 'F', 'Actress', '2023-03-02 13:53:40'),
(11, 'Dennis Adhiswara', '1982-09-14', 'Malang', 'M', 'Actor', '2023-03-26 00:00:00');

-- --------------------------------------------------------

--
-- Table structure for table `patient_bpjs`
--

CREATE TABLE `patient_bpjs` (
  `patient_id` int(11) NOT NULL,
  `bpjs_no` varchar(64) NOT NULL,
  `registered_date` date NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `patient_bpjs`
--

INSERT INTO `patient_bpjs` (`patient_id`, `bpjs_no`, `registered_date`) VALUES
(9, '123123123', '2023-04-03'),
(5, '123123123213', '2023-04-10');

-- --------------------------------------------------------

--
-- Table structure for table `rooms`
--

CREATE TABLE `rooms` (
  `id` int(11) NOT NULL,
  `name` varchar(100) NOT NULL,
  `type_id` smallint(5) UNSIGNED DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Table structure for table `room_type`
--

CREATE TABLE `room_type` (
  `type_id` smallint(5) UNSIGNED NOT NULL COMMENT '\r\n',
  `code` varchar(10) DEFAULT NULL,
  `name` varchar(50) DEFAULT NULL,
  `daily_price` decimal(12,2) DEFAULT NULL,
  `is_inpatient` tinyint(1) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `room_type`
--

INSERT INTO `room_type` (`type_id`, `code`, `name`, `daily_price`, `is_inpatient`) VALUES
(1, 'IGD', 'Instalasi Gawat Darurat', '0.00', 0),
(2, 'RKDO', 'Ruang Konsultasi Dokter Orthopedi', '0.00', 0),
(3, 'RTDO', 'Ruang Tindakan Dokter Orthopedi', '0.00', 0),
(4, 'RIR', 'Ruang Instalasi Radiologi', '0.00', 0),
(5, 'VIP', 'Kamar VIP', '500000.00', 1),
(6, 'CLS1', 'Kamar Kelas I', '300000.00', 1),
(7, 'CLS2', 'Kamar Kelas II', '200000.00', 1),
(8, 'CLS3', 'Kamar Kelas III', '100000.00', 1),
(9, 'FRM', 'Farmasi', '0.00', 0);

-- --------------------------------------------------------

--
-- Structure for view `daftar_dokter`
--
DROP TABLE IF EXISTS `daftar_dokter`;

CREATE ALGORITHM=UNDEFINED DEFINER=`root`@`localhost` SQL SECURITY DEFINER VIEW `daftar_dokter`  AS SELECT `a`.`id` AS `ID`, `a`.`fullname` AS `Nama Lengkap`, `a`.`nik` AS `NIK`, `a`.`birth_date` AS `Tanggal Lahir`, `a`.`birth_place` AS `Tempat Lahir`, `a`.`sex` AS `Jenis Kelamin`, `b`.`type` AS `Spesialisasi` FROM (`doctors` `a` join `doctor_type` `b` on(`a`.`type_id` = `b`.`id`))  ;

-- --------------------------------------------------------

--
-- Structure for view `data_obat`
--
DROP TABLE IF EXISTS `data_obat`;

CREATE ALGORITHM=UNDEFINED DEFINER=`root`@`localhost` SQL SECURITY DEFINER VIEW `data_obat`  AS SELECT `a`.`id` AS `ID`, `a`.`name` AS `Nama Obat`, `a`.`diagnose` AS `Indikasi`, `a`.`quantity` AS `Kuantitas`, `a`.`price` AS `Harga` FROM `drugs` AS `a``a`  ;

-- --------------------------------------------------------

--
-- Structure for view `data_pasien`
--
DROP TABLE IF EXISTS `data_pasien`;

CREATE ALGORITHM=UNDEFINED DEFINER=`root`@`localhost` SQL SECURITY DEFINER VIEW `data_pasien`  AS SELECT `a`.`id` AS `ID`, `a`.`fullname` AS `Nama Lengkap`, `a`.`birth_date` AS `Tanggal Lahir`, `a`.`birth_place` AS `Tempat Lahir`, `a`.`sex` AS `Jenis Kelamin`, `a`.`occupation` AS `Pekerjaan` FROM `patients` AS `a``a`  ;

--
-- Indexes for dumped tables
--

--
-- Indexes for table `checkup`
--
ALTER TABLE `checkup`
  ADD PRIMARY KEY (`id`),
  ADD KEY `checkup_ibfk_1` (`doctor_id`),
  ADD KEY `patient_id` (`patient_id`);

--
-- Indexes for table `checkup_medicine`
--
ALTER TABLE `checkup_medicine`
  ADD PRIMARY KEY (`checkup_id`,`drug_id`),
  ADD KEY `drug_id` (`drug_id`);

--
-- Indexes for table `doctors`
--
ALTER TABLE `doctors`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `email` (`email`),
  ADD KEY `type_id` (`type_id`);

--
-- Indexes for table `doctor_schedule`
--
ALTER TABLE `doctor_schedule`
  ADD PRIMARY KEY (`id`);

--
-- Indexes for table `doctor_type`
--
ALTER TABLE `doctor_type`
  ADD PRIMARY KEY (`id`);

--
-- Indexes for table `drugs`
--
ALTER TABLE `drugs`
  ADD PRIMARY KEY (`id`);

--
-- Indexes for table `patients`
--
ALTER TABLE `patients`
  ADD PRIMARY KEY (`id`);

--
-- Indexes for table `patient_bpjs`
--
ALTER TABLE `patient_bpjs`
  ADD KEY `patient_id` (`patient_id`);

--
-- Indexes for table `rooms`
--
ALTER TABLE `rooms`
  ADD PRIMARY KEY (`id`),
  ADD KEY `type_id` (`type_id`);

--
-- Indexes for table `room_type`
--
ALTER TABLE `room_type`
  ADD PRIMARY KEY (`type_id`);

--
-- AUTO_INCREMENT for dumped tables
--

--
-- AUTO_INCREMENT for table `checkup`
--
ALTER TABLE `checkup`
  MODIFY `id` bigint(20) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=11;

--
-- AUTO_INCREMENT for table `doctors`
--
ALTER TABLE `doctors`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=7;

--
-- AUTO_INCREMENT for table `doctor_schedule`
--
ALTER TABLE `doctor_schedule`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT for table `doctor_type`
--
ALTER TABLE `doctor_type`
  MODIFY `id` smallint(5) UNSIGNED NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=10;

--
-- AUTO_INCREMENT for table `drugs`
--
ALTER TABLE `drugs`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=23;

--
-- AUTO_INCREMENT for table `patients`
--
ALTER TABLE `patients`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=12;

--
-- AUTO_INCREMENT for table `rooms`
--
ALTER TABLE `rooms`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT for table `room_type`
--
ALTER TABLE `room_type`
  MODIFY `type_id` smallint(5) UNSIGNED NOT NULL AUTO_INCREMENT COMMENT '\r\n', AUTO_INCREMENT=10;

--
-- Constraints for dumped tables
--

--
-- Constraints for table `checkup`
--
ALTER TABLE `checkup`
  ADD CONSTRAINT `checkup_ibfk_1` FOREIGN KEY (`doctor_id`) REFERENCES `doctors` (`id`) ON UPDATE CASCADE,
  ADD CONSTRAINT `checkup_ibfk_2` FOREIGN KEY (`patient_id`) REFERENCES `patients` (`id`) ON UPDATE CASCADE;

--
-- Constraints for table `checkup_medicine`
--
ALTER TABLE `checkup_medicine`
  ADD CONSTRAINT `checkup_medicine_ibfk_1` FOREIGN KEY (`checkup_id`) REFERENCES `checkup` (`id`),
  ADD CONSTRAINT `checkup_medicine_ibfk_2` FOREIGN KEY (`drug_id`) REFERENCES `drugs` (`id`);

--
-- Constraints for table `doctors`
--
ALTER TABLE `doctors`
  ADD CONSTRAINT `doctors_ibfk_1` FOREIGN KEY (`type_id`) REFERENCES `doctor_type` (`id`) ON UPDATE CASCADE;

--
-- Constraints for table `patient_bpjs`
--
ALTER TABLE `patient_bpjs`
  ADD CONSTRAINT `patient_bpjs_ibfk_1` FOREIGN KEY (`patient_id`) REFERENCES `patients` (`id`);

--
-- Constraints for table `rooms`
--
ALTER TABLE `rooms`
  ADD CONSTRAINT `rooms_ibfk_1` FOREIGN KEY (`type_id`) REFERENCES `room_type` (`type_id`) ON UPDATE CASCADE;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
