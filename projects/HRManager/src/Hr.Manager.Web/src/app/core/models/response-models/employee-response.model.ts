import { Gender } from "../enums/gender.enum";
import { AddressModel } from "../value-objects/address.model";


export interface EmployeeResponse {
    id: string;
    firstName: string;
    lastName: string;
    middleName: string;
    email: string;
    passportNumber: string;
    dateOfBirth: string;
    nationality: string;
    gender: Gender;
    pinfl: string;
    pensionFundNumber: string;
    taxIdentificationNumber: string;
    phoneNumber: string;
    address: AddressModel;
    hireDate: string;
    terminationDate?: string | null;
    departmentId: string;
    departmentName: string;
    positionId: string;
    positionName: string;
    salary: number;
    scheduleId: string;
    isManagerOfDepartment: boolean;
}

export interface EmployeesBriefResponse {
    id: string;
    fullName: string;
    departmentName: string;
    positionName: string;
    email: string;
    phoneNumber: string;
    hireDate: string;
    isManagerOfDepartment: boolean;
}