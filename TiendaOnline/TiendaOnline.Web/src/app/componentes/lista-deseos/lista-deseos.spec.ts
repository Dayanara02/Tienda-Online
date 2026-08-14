import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ListaDeseos } from './lista-deseos';

describe('ListaDeseos', () => {
  let component: ListaDeseos;
  let fixture: ComponentFixture<ListaDeseos>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ListaDeseos],
    }).compileComponents();

    fixture = TestBed.createComponent(ListaDeseos);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
