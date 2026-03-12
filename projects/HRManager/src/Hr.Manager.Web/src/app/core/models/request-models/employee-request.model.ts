import { Gender } from "../enums/gender.enum";
import { AddressModel } from "../value-objects/address.model";

export interface CreateEmployeeRequest {
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
    positionId: string;
    salary: number;
    scheduleId: string;
}

export interface UpdateEmployeeRequest extends CreateEmployeeRequest {
    id: string;
}

export interface GetEmployeeByEmailRequest {
    email: string;
}

export interface DeleteEmployeeRequest {
    id: string;
}

export interface GetEmployeeByIdRequest {
    id: string;
}
