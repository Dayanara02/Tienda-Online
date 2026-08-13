import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EmpleadoDashboard } from './empleado-dashboard';

describe('EmpleadoDashboard', () => {
  let component: EmpleadoDashboard;
  let fixture: ComponentFixture<EmpleadoDashboard>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [EmpleadoDashboard],
    }).compileComponents();

    fixture = TestBed.createComponent(EmpleadoDashboard);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
